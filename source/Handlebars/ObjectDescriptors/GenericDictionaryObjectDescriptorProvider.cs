using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.MemberAccessors;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal sealed class GenericDictionaryObjectDescriptorProvider : IObjectDescriptorProvider
    {
        private static readonly object[] EmptyArray = new object[0];
        
        private readonly RefLookup<Type, DeferredValue<Type>> _typeCache = new RefLookup<Type, DeferredValue<Type>>();

        public bool CanHandleType(Type type)
        {
            ref var deferredValue = ref _typeCache.GetOrAdd(type, InterfaceTypeValueFactory);
            return deferredValue.Value != null;
        }

        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            value = null;
            var interfaceType = _typeCache.GetOrAdd(type, InterfaceTypeValueFactory).Value;
            if (interfaceType == null) return false;
            
            var descriptorCreator = GetType().GetMethod(nameof(CreateDescriptor), BindingFlags.NonPublic | BindingFlags.Static)
                ?.MakeGenericMethod(interfaceType.GetGenericArguments());

            value = (ObjectDescriptor) descriptorCreator?.Invoke(null, EmptyArray);
            return value != null;
        }
        
        private static ref DeferredValue<Type> InterfaceTypeValueFactory(Type type, ref DeferredValue<Type> deferredValue)
        {
            deferredValue.Factory = () =>
            {
                return type.GetInterfaces()
                    .Where(i => i.GetTypeInfo().IsGenericType)
                    .Where(i => i.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                    .FirstOrDefault(i => 
                        TypeDescriptor.GetConverter(i.GetGenericArguments()[0]).CanConvertFrom(typeof(string))
                    );
            };
            
            return ref deferredValue;
        }

        private static ObjectDescriptor CreateDescriptor<T, TV>()
        {
            return new ObjectDescriptor(typeof(IDictionary<T, TV>))
            {
                GetProperties = o => ((IDictionary<T, TV>) o).Keys.Cast<object>(),
                MemberAccessor = new DictionaryAccessor<T, TV>()
            };
        }
        
        private class DictionaryAccessor<T, TV> : IMemberAccessor
        {
            private static readonly TypeConverter TypeConverter = TypeDescriptor.GetConverter(typeof(T));

            public bool TryGetValue(object instance, Type instanceType, string memberName, out object value)
            {
                var key = (T) TypeConverter.ConvertFromString(memberName);
                var dictionary = (IDictionary<T, TV>) instance;
                if (dictionary.TryGetValue(key, out var v))
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