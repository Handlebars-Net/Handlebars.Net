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
                ? (IMemberAccessor) new CompiledReflectionMemberAccessor()
                : (IMemberAccessor) new PlainReflectionMemberAccessor();
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

        private class PlainReflectionMemberAccessor : IMemberAccessor
        {
            private readonly LookupSlim<Type, DeferredValue<Type, RawObjectTypeDescriptor>> _descriptors =
                new LookupSlim<Type, DeferredValue<Type, RawObjectTypeDescriptor>>();

            private static readonly Func<Type, DeferredValue<Type, RawObjectTypeDescriptor>> ValueFactory = 
                key => new DeferredValue<Type, RawObjectTypeDescriptor>(key, type => new RawObjectTypeDescriptor(type));

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

            private class RawObjectTypeDescriptor
            {
                private readonly Type _type;

                private static readonly MethodInfo CreateGetDelegateMethodInfo = typeof(RawObjectTypeDescriptor)
                    .GetMethod(nameof(CreateGetDelegate), BindingFlags.Static | BindingFlags.NonPublic);

                private static readonly Func<(string key, Type type), Func<object, object>> ValueGetterFactory =
                    o => GetValueGetter(o.key, o.type);

                private readonly LookupSlim<string, DeferredValue<(string key, Type type), Func<object, object>>>
                    _accessors = new LookupSlim<string, DeferredValue<(string key, Type type), Func<object, object>>>();

                private static readonly Func<string, Type, DeferredValue<(string key, Type type), Func<object, object>>> ValueFactory =
                    (key, state) => new DeferredValue<(string key, Type type), Func<object, object>>((key, state), ValueGetterFactory);

                public RawObjectTypeDescriptor(Type type)
                {
                    _type = type;
                }

                public Func<object, object> GetOrCreateAccessor(string name)
                {
                    return _accessors.TryGetValue(name, out var deferredValue)
                        ? deferredValue.Value
                        : _accessors.GetOrAdd(name, ValueFactory, _type).Value;
                }

                private static Func<object, object> GetValueGetter(string name, Type type)
                {
                    var property = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .FirstOrDefault(o =>
                            o.GetIndexParameters().Length == 0 &&
                            string.Equals(o.Name, name, StringComparison.OrdinalIgnoreCase));
                    ;

                    if (property != null)
                    {
                        return (Func<object, object>) CreateGetDelegateMethodInfo.MakeGenericMethod(type, property.PropertyType)
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
        }

        private class CompiledReflectionMemberAccessor : IMemberAccessor
        {
            private readonly LookupSlim<Type, DeferredValue<Type, CompiledObjectTypeDescriptor>> _descriptors =
                new LookupSlim<Type, DeferredValue<Type, CompiledObjectTypeDescriptor>>();

            private static readonly Func<Type, DeferredValue<Type, CompiledObjectTypeDescriptor>> ValueFactory =
                    key => new DeferredValue<Type, CompiledObjectTypeDescriptor>(key, type => new CompiledObjectTypeDescriptor(type));

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

            private class CompiledObjectTypeDescriptor
            {
                private readonly Type _type;

                private static readonly Func<(string key, Type type), Func<object, object>> ValueGetterFactory =
                    o => GetValueGetter(o.key, o.type);

                private readonly LookupSlim<string, DeferredValue<(string key, Type type), Func<object, object>>>
                    _accessors =
                        new LookupSlim<string, DeferredValue<(string key, Type type), Func<object, object>>>();

                private static readonly Func<string, Type, DeferredValue<(string key, Type type), Func<object, object>>> ValueFactory =
                    (key, state) => new DeferredValue<(string key, Type type), Func<object, object>>((key, state), ValueGetterFactory);

                public CompiledObjectTypeDescriptor(Type type)
                {
                    _type = type;
                }

                public Func<object, object> GetOrCreateAccessor(string name)
                {
                    return _accessors.TryGetValue(name, out var deferredValue)
                        ? deferredValue.Value
                        : _accessors.GetOrAdd(name, ValueFactory, _type).Value;
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
}