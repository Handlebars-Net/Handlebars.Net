using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HandlebarsDotNet.Collections;

namespace HandlebarsDotNet.MemberAccessors
{
    internal class ReflectionMemberAccessor : IMemberAccessor
    {
        private readonly InternalHandlebarsConfiguration _configuration;
        private readonly IMemberAccessor _inner;

        public ReflectionMemberAccessor(InternalHandlebarsConfiguration configuration)
        {
            _configuration = configuration;
            _inner = configuration.CompileTimeConfiguration.UseAggressiveCaching
                ? (IMemberAccessor) new MemberAccessor<CompiledObjectTypeDescriptor>()
                : (IMemberAccessor) new MemberAccessor<RawObjectTypeDescriptor>();
        }

        public bool TryGetValue(object instance, Type instanceType, string memberName, out object value)
        {
            if (_inner.TryGetValue(instance, instanceType, memberName, out value))
            {
                return true;
            }

            var aliasProviders = _configuration.CompileTimeConfiguration.AliasProviders;
            for (var index = 0; index < aliasProviders.Count; index++)
            {
                if (aliasProviders[index].TryGetMemberByAlias(instance, instanceType, memberName, out value))
                    return true;
            }

            value = null;
            return false;
        }

        private abstract class ObjectTypeDescriptor
        {
            protected readonly LookupSlim<string, DeferredValue<(string key, Type type), Func<object, object>>>
                Accessors = new LookupSlim<string, DeferredValue<(string key, Type type), Func<object, object>>>();
            
            protected Type Type { get; }

            public ObjectTypeDescriptor(Type type)
            {
                Type = type;
            }

            public abstract Func<object, object> GetOrCreateAccessor(string name);
        }

        private class MemberAccessor<T> : IMemberAccessor
            where T : ObjectTypeDescriptor
        {
            private readonly LookupSlim<Type, DeferredValue<Type, T>> _descriptors =
                new LookupSlim<Type, DeferredValue<Type, T>>();

            private static readonly Func<Type, DeferredValue<Type, T>> ValueFactory =
                key => new DeferredValue<Type, T>(key, type => (T) Activator.CreateInstance(typeof(T), type));

            public bool TryGetValue(object instance, Type instanceType, string memberName, out object value)
            {
                if (!_descriptors.TryGetValue(instanceType, out var deferredValue))
                {
                    deferredValue = _descriptors.GetOrAdd(instanceType, ValueFactory);
                }

                var accessor = deferredValue.Value.GetOrCreateAccessor(memberName);
                value = accessor?.Invoke(instance);
                return accessor != null;
            }
        }

        private sealed class RawObjectTypeDescriptor : ObjectTypeDescriptor
        {
            private static readonly MethodInfo CreateGetDelegateMethodInfo = typeof(RawObjectTypeDescriptor)
                .GetMethod(nameof(CreateGetDelegate), BindingFlags.Static | BindingFlags.NonPublic);

            private static readonly Func<(string key, Type type), Func<object, object>> ValueGetterFactory = o => GetValueGetter(o.key, o.type);

            private static readonly Func<string, Type, DeferredValue<(string key, Type type), Func<object, object>>>
                ValueFactory = (key, state) => new DeferredValue<(string key, Type type), Func<object, object>>((key, state), ValueGetterFactory);

            public RawObjectTypeDescriptor(Type type) : base(type)
            {
            }

            public override Func<object, object> GetOrCreateAccessor(string name)
            {
                return Accessors.TryGetValue(name, out var deferredValue)
                    ? deferredValue.Value
                    : Accessors.GetOrAdd(name, ValueFactory, Type).Value;
            }

            private static Func<object, object> GetValueGetter(string name, Type type)
            {
                var property = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(o =>
                        o.GetIndexParameters().Length == 0 &&
                        string.Equals(o.Name, name, StringComparison.OrdinalIgnoreCase));

                if (property != null)
                {
                    return (Func<object, object>) CreateGetDelegateMethodInfo
                        .MakeGenericMethod(type, property.PropertyType)
                        .Invoke(null, new[] {property});
                }

                var field = type.GetFields(BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(o => string.Equals(o.Name, name, StringComparison.OrdinalIgnoreCase));
                if (field != null)
                {
                    return o => field.GetValue(o);
                }

                return null;
            }

            private static Func<object, object> CreateGetDelegate<T, TValue>(PropertyInfo property)
            {
                var @delegate = (Func<T, TValue>) property.GetMethod.CreateDelegate(typeof(Func<T, TValue>));
                return o => (object) @delegate((T) o);
            }
        }

        private sealed class CompiledObjectTypeDescriptor : ObjectTypeDescriptor
        {
            private static readonly Func<(string key, Type type), Func<object, object>> ValueGetterFactory =
                o => GetValueGetter(o.key, o.type);

            private static readonly Func<string, Type, DeferredValue<(string key, Type type), Func<object, object>>>
                ValueFactory = (key, state) => new DeferredValue<(string key, Type type), Func<object, object>>((key, state), ValueGetterFactory);

            public CompiledObjectTypeDescriptor(Type type) : base(type)
            {
            }

            public override Func<object, object> GetOrCreateAccessor(string name)
            {
                return Accessors.TryGetValue(name, out var deferredValue)
                    ? deferredValue.Value
                    : Accessors.GetOrAdd(name, ValueFactory, Type).Value;
            }

            private static Func<object, object> GetValueGetter(string name, Type type)
            {
                var property = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(o =>
                        o.GetIndexParameters().Length == 0 &&
                        string.Equals(o.Name, name, StringComparison.OrdinalIgnoreCase));

                if (property != null)
                {
                    var instance = Expression.Parameter(typeof(object), "i");
                    var memberExpression = Expression.Property(Expression.Convert(instance, type), name);
                    var convert = Expression.TypeAs(memberExpression, typeof(object));

                    return (Func<object, object>) Expression.Lambda(convert, instance).Compile();
                }

                var field = type.GetFields(BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(o => string.Equals(o.Name, name, StringComparison.OrdinalIgnoreCase));

                if (field != null)
                {
                    var instance = Expression.Parameter(typeof(object), "i");
                    var memberExpression = Expression.Field(Expression.Convert(instance, type), name);
                    var convert = Expression.TypeAs(memberExpression, typeof(object));

                    return (Func<object, object>) Expression.Lambda(convert, instance).Compile();
                }

                return null;
            }
        }
    }
}