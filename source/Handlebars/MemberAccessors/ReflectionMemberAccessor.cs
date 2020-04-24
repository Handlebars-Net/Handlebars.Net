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
                if (aliasProviders[index].TryGetMemberByAlias(instance, instanceType, memberName, out value)) return true;
            }

            value = null;
            return false;
        }

        private class PlainReflectionMemberAccessor : IMemberAccessor
        {
            private readonly RefLookup<Type, DeferredValue<ObjectTypeDescriptor>> _descriptors =
                new RefLookup<Type, DeferredValue<ObjectTypeDescriptor>>();

            public bool TryGetValue(object instance, Type instanceType, string memberName, out object value)
            {
                ObjectTypeDescriptor descriptor;
                if (_descriptors.ContainsKey(instanceType))
                {
                    ref var deferredValue = ref _descriptors.GetValueOrDefault(instanceType);
                    descriptor = deferredValue.Value;
                }else
                {
                    ref var deferredValue = ref _descriptors.GetOrAdd(instanceType, ValueFactory);
                    descriptor = deferredValue.Value;
                }

                var accessor = descriptor.GetOrCreateAccessor(memberName);
                value = accessor?.Invoke(instance);
                return accessor != null;
            }

            private static ref DeferredValue<ObjectTypeDescriptor> ValueFactory(Type type, ref DeferredValue<ObjectTypeDescriptor> deferredValue)
            {
                deferredValue.Factory = () => new RawObjectTypeDescriptor(type);
                return ref deferredValue;
            }
        }

        private class CompiledReflectionMemberAccessor : IMemberAccessor
        {
            private readonly RefLookup<Type, DeferredValue<ObjectTypeDescriptor>> _descriptors =
                new RefLookup<Type, DeferredValue<ObjectTypeDescriptor>>();

            public bool TryGetValue(object instance, Type instanceType, string memberName, out object value)
            {
                ObjectTypeDescriptor descriptor;
                if (_descriptors.ContainsKey(instanceType))
                {
                    ref var deferredValue = ref _descriptors.GetValueOrDefault(instanceType);
                    descriptor = deferredValue.Value;
                }
                else
                {
                    ref var deferredValue = ref _descriptors.GetOrAdd(instanceType, ValueFactory);
                    descriptor = deferredValue.Value;   
                }

                var accessor = descriptor.GetOrCreateAccessor(memberName);
                value = accessor?.Invoke(instance);
                return accessor != null;
            }

            private static ref DeferredValue<ObjectTypeDescriptor> ValueFactory(Type type, ref DeferredValue<ObjectTypeDescriptor> deferredValue)
            {
                deferredValue.Factory = () => new CompiledObjectTypeDescriptor(type);
                return ref deferredValue;
            }
        }

        private class CompiledObjectTypeDescriptor : ObjectTypeDescriptor
        {
            public CompiledObjectTypeDescriptor(Type type) : base(type)
            {
            }

            protected override Func<object, object> GetValueGetter(string name, Type type)
            {
                var property = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(o => o.GetIndexParameters().Length == 0 && string.Equals(o.Name, name, StringComparison.OrdinalIgnoreCase));
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

        private class RawObjectTypeDescriptor : ObjectTypeDescriptor
        {
            public RawObjectTypeDescriptor(Type type) : base(type)
            {
            }

            protected override Func<object, object> GetValueGetter(string name, Type type)
            {
                var property = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(o => o.GetIndexParameters().Length == 0 && string.Equals(o.Name, name, StringComparison.OrdinalIgnoreCase));;
                if (property != null)
                {
                    return o => property.GetValue(o);
                }

                var field = type.GetFields(BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(o => string.Equals(o.Name, name, StringComparison.OrdinalIgnoreCase));
                if (field != null)
                {
                    return o => field.GetValue(o);
                }

                return null;
            }
        }

        private abstract class ObjectTypeDescriptor
        {
            private readonly Type _type;

            private readonly RefLookup<string, DeferredValue<Func<object, object>>> _accessors =
                new RefLookup<string, DeferredValue<Func<object, object>>>();

            protected ObjectTypeDescriptor(Type type)
            {
                _type = type;
            }

            public Func<object, object> GetOrCreateAccessor(string name)
            {
                if (_accessors.ContainsKey(name))
                {
                    ref var existing = ref _accessors.GetValueOrDefault(name);
                    return existing.Value;
                }
                
                ref var deferredValue = ref _accessors.GetOrAdd(name, ValueFactory);
                return deferredValue.Value;
            }

            private ref DeferredValue<Func<object, object>> ValueFactory(string name, ref DeferredValue<Func<object, object>> deferredValue)
            {
                deferredValue.Factory = () => GetValueGetter(name, _type);
                return ref deferredValue;
            }

            protected abstract Func<object, object> GetValueGetter(string name, Type type);
        }
    }
}