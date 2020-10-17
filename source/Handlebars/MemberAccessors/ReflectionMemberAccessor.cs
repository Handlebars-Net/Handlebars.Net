using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.MemberAccessors
{
    internal sealed class ReflectionMemberAccessor : IMemberAccessor
    {
        private readonly LookupSlim<Type, DeferredValue<Type, RawObjectTypeDescriptor>> _descriptors =
            new LookupSlim<Type, DeferredValue<Type, RawObjectTypeDescriptor>>();

        private static readonly Func<Type, DeferredValue<Type, RawObjectTypeDescriptor>> DescriptorsValueFactory =
            key => new DeferredValue<Type, RawObjectTypeDescriptor>(key, type => new RawObjectTypeDescriptor(type));
        
        private readonly ICompiledHandlebarsConfiguration _configuration;

        public ReflectionMemberAccessor(ICompiledHandlebarsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool TryGetValue(object instance, ChainSegment memberName, out object value)
        {
            var instanceType = instance.GetType();
            if (TryGetValueImpl(instance, instanceType, memberName, out value)) return true;

            var aliasProviders = _configuration.AliasProviders;
            for (var index = 0; index < aliasProviders.Count; index++)
            {
                if (aliasProviders[index].TryGetMemberByAlias(instance, instanceType, memberName, out value))
                    return true;
            }

            value = null;
            return false;
        }

        private bool TryGetValueImpl(object instance, Type instanceType, ChainSegment memberName, out object value)
        {
            if (!_descriptors.TryGetValue(instanceType, out var deferredValue))
            {
                deferredValue = _descriptors.GetOrAdd(instanceType, DescriptorsValueFactory);
            }

            var accessor = deferredValue.Value.GetOrCreateAccessor(memberName);
            value = accessor?.Invoke(instance);
            return accessor != null;
        }

        private sealed class RawObjectTypeDescriptor
        {
            private static readonly MethodInfo CreateGetDelegateMethodInfo = typeof(RawObjectTypeDescriptor)
                .GetMethod(nameof(CreateGetDelegate), BindingFlags.Static | BindingFlags.NonPublic);

            private static readonly Func<KeyValuePair<ChainSegment, Type>, Func<object, object>> ValueGetterFactory = o => GetValueGetter(o.Key, o.Value);

            private static readonly Func<ChainSegment, Type, DeferredValue<KeyValuePair<ChainSegment, Type>, Func<object, object>>>
                ValueFactory = (key, state) => new DeferredValue<KeyValuePair<ChainSegment, Type>, Func<object, object>>(new KeyValuePair<ChainSegment, Type>(key, state), ValueGetterFactory);

            private readonly LookupSlim<ChainSegment, DeferredValue<KeyValuePair<ChainSegment, Type>, Func<object, object>>>
                _accessors = new LookupSlim<ChainSegment, DeferredValue<KeyValuePair<ChainSegment, Type>, Func<object, object>>>();

            private Type Type { get; }

            public RawObjectTypeDescriptor(Type type)
            {
                Type = type;
            }

            public Func<object, object> GetOrCreateAccessor(ChainSegment name)
            {
                return _accessors.TryGetValue(name, out var deferredValue)
                    ? deferredValue.Value
                    : _accessors.GetOrAdd(name, ValueFactory, Type).Value;
            }

            private static Func<object, object> GetValueGetter(ChainSegment name, Type type)
            {
                var property = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(o =>
                        o.GetIndexParameters().Length == 0 &&
                        string.Equals(o.Name, name.LowerInvariant, StringComparison.OrdinalIgnoreCase)
                );

                if (property != null)
                {
                    return (Func<object, object>) CreateGetDelegateMethodInfo
                        .MakeGenericMethod(type, property.PropertyType)
                        .Invoke(null, new object[] {property});
                }

                var field = type.GetFields(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(o => string.Equals(o.Name, name.LowerInvariant, StringComparison.OrdinalIgnoreCase));
                
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
}