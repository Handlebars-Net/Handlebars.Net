using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.MemberAccessors;
using HandlebarsDotNet.Polyfills;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal sealed class GenericDictionaryObjectDescriptorProvider : IObjectDescriptorProvider
    {
        private static readonly MethodInfo CreateDescriptorMethodInfo = typeof(GenericDictionaryObjectDescriptorProvider)
            .GetMethod(nameof(CreateDescriptor), BindingFlags.NonPublic | BindingFlags.Static);
        
        private readonly LookupSlim<Type, DeferredValue<Type, Type>> _typeCache = new LookupSlim<Type, DeferredValue<Type, Type>>();

        public bool CanHandleType(Type type)
        {
            var deferredValue = _typeCache.GetOrAdd(type, InterfaceTypeValueFactory);
            return deferredValue.Value != null;
        }

        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            var interfaceType = _typeCache.GetOrAdd(type, InterfaceTypeValueFactory).Value;
            if (interfaceType == null)
            {
                value = ObjectDescriptor.Empty;
                return false;
            }

            var descriptorCreator = CreateDescriptorMethodInfo
                .MakeGenericMethod(interfaceType.GetGenericArguments());
                    
            value = (ObjectDescriptor) descriptorCreator.Invoke(null, ArrayEx.Empty<object>());
            return true;
        }

        private static readonly Func<Type, DeferredValue<Type, Type>> InterfaceTypeValueFactory =
            key => new DeferredValue<Type, Type>(key, type =>
            {
                return type.GetInterfaces()
                    .Where(i => i.GetTypeInfo().IsGenericType)
                    .Where(i => i.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                    .FirstOrDefault(i =>
                        TypeDescriptor.GetConverter(i.GetGenericArguments()[0]).CanConvertFrom(typeof(string))
                    );
            });

        private static ObjectDescriptor CreateDescriptor<T, TV>()
        {
            IEnumerable<object> Enumerate(IDictionary<T, TV> o)
            {
                foreach (var key in o.Keys) yield return key;
            }
            
            return new ObjectDescriptor(
                typeof(IDictionary<T, TV>), 
                new DictionaryAccessor<T, TV>(),
                (descriptor, o) => Enumerate((IDictionary<T, TV>) o)
            );
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