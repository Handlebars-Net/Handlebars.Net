using System;
using System.Collections.Generic;
using System.Reflection;
using HandlebarsDotNet.MemberAccessors;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal sealed class KeyValuePairObjectDescriptorProvider : IObjectDescriptorProvider
    {
        private static readonly string[] Properties = { "key", "value" };
        private static readonly object[] EmptyArray = new object[0];
        
        public bool CanHandleType(Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
        }

        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            var genericArguments = type.GetGenericArguments();
            var descriptorCreator = GetType().GetMethod(nameof(CreateDescriptor), BindingFlags.NonPublic | BindingFlags.Static)
                ?.MakeGenericMethod(genericArguments[0], genericArguments[1]);

            value = (ObjectDescriptor) descriptorCreator?.Invoke(null, EmptyArray);
            return value != null;
        }
        
        private static ObjectDescriptor CreateDescriptor<T, TV>()
        {
            return new ObjectDescriptor(typeof(KeyValuePair<T, TV>))
            {
                GetProperties = o => Properties,
                MemberAccessor = new KeyValuePairAccessor<T, TV>()
            };
        }
        
        private class KeyValuePairAccessor<T, TV> : IMemberAccessor
        {
            public bool TryGetValue(object instance, Type instanceType, string memberName, out object value)
            {
                var keyValuePair = (KeyValuePair<T, TV>) instance;
                switch (memberName.ToLowerInvariant())
                {
                    case "key":
                        value = keyValuePair.Key;
                        return true;
                    
                    case "value":
                        value = keyValuePair.Value;
                        return true;
                    
                    default:
                        value = default(TV);
                        return false;
                }
            }
        }
    }
}