using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.MemberAccessors;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal sealed class StringDictionaryObjectDescriptorProvider : IObjectDescriptorProvider
    {
        private readonly RefLookup<Type, DeferredValue<Type>> _typeCache = new RefLookup<Type, DeferredValue<Type>>();
        private static readonly object[] EmptyArray = new object[0];

        public bool CanHandleType(Type type)
        {
            ref var deferredValue = ref _typeCache.GetOrAdd(type, InterfaceTypeValueFactory);
            return deferredValue.Value != null;
        }

        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            value = null;
            ref var deferredValue = ref _typeCache.GetOrAdd(type, InterfaceTypeValueFactory);
            var interfaceType = deferredValue.Value;
            
            if (interfaceType == null) return false;

            var descriptorCreator = GetType().GetMethod(nameof(CreateDescriptor), BindingFlags.NonPublic | BindingFlags.Static)
                ?.MakeGenericMethod(interfaceType.GetGenericArguments()[1]);

            value = (ObjectDescriptor) descriptorCreator?.Invoke(null, EmptyArray);
            return value != null;
        }
        
        private static ref DeferredValue<Type> InterfaceTypeValueFactory(Type type, ref DeferredValue<Type> deferredValue)
        {
            deferredValue.Factory = () =>
            {
                return type.GetInterfaces()
                    .FirstOrDefault(i =>
                        i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>) &&
                        i.GetGenericArguments()[0] == typeof(string));
            };
            
            return ref deferredValue;
        }

        private static ObjectDescriptor CreateDescriptor<TV>()
        {
            return new ObjectDescriptor(typeof(IDictionary<string, TV>))
            {
                GetProperties = o => ((IDictionary<string, TV>) o).Keys,
                MemberAccessor = new DictionaryAccessor<TV>()
            };
        }
        
        private class DictionaryAccessor<TV> : IMemberAccessor
        {
            public bool TryGetValue(object instance, Type instanceType, string memberName, out object value)
            {
                var dictionary = (IDictionary<string, TV>) instance;
                if (dictionary.TryGetValue(memberName, out var v))
                {
                    value = v;
                    return true;
                }

                value = default(TV);
                return false;
            }
        }
    }
}