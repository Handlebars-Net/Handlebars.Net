using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.MemberAccessors;
using HandlebarsDotNet.Polyfills;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal sealed class StringDictionaryObjectDescriptorProvider : IObjectDescriptorProvider
    {
        private static readonly object[] EmptyArray = ArrayEx.Empty<object>();
        private static readonly MethodInfo CreateDescriptorMethodInfo = typeof(StringDictionaryObjectDescriptorProvider).GetMethod(nameof(CreateDescriptor), BindingFlags.NonPublic | BindingFlags.Static);
        
        private readonly LookupSlim<Type, DeferredValue<Type, Type>> _typeCache = new LookupSlim<Type, DeferredValue<Type, Type>>();

        public bool CanHandleType(Type type)
        {
            return _typeCache.GetOrAdd(type, InterfaceTypeValueFactory).Value != null;
        }

        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            var interfaceType = _typeCache.TryGetValue(type, out var deferredValue) 
                ? deferredValue.Value 
                : _typeCache.GetOrAdd(type, InterfaceTypeValueFactory).Value;
            
            if (interfaceType == null)
            {
                value = ObjectDescriptor.Empty;
                return false;
            }

            var descriptorCreator = CreateDescriptorMethodInfo
                .MakeGenericMethod(interfaceType.GetGenericArguments()[1]);

            value = (ObjectDescriptor) descriptorCreator.Invoke(null, EmptyArray);
            return true;
        }
        
        private static readonly Func<Type, DeferredValue<Type, Type>> InterfaceTypeValueFactory = 
            key => new DeferredValue<Type, Type>(key, type =>
            {
                return type.GetInterfaces()
                    .FirstOrDefault(i =>
                        i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>) &&
                        i.GetGenericArguments()[0] == typeof(string));
            });

        private static ObjectDescriptor CreateDescriptor<TV>()
        {
            return new ObjectDescriptor(
                typeof(IDictionary<string, TV>),
                new DictionaryAccessor<TV>(),
                (descriptor, o) => ((IDictionary<string, TV>) o).Keys
            );
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