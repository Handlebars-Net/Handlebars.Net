using System;
using System.Collections.Generic;
using System.Reflection;
using HandlebarsDotNet.MemberAccessors;
using HandlebarsDotNet.Polyfills;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal sealed class KeyValuePairObjectDescriptorProvider : IObjectDescriptorProvider
    {
        private static readonly string[] Properties = { "key", "value" };
        private static readonly MethodInfo CreateDescriptorMethodInfo = typeof(KeyValuePairObjectDescriptorProvider).GetMethod(nameof(CreateDescriptor), BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly Func<ObjectDescriptor, object, IEnumerable<object>> GetProperties = (descriptor, o) => Properties;

        public bool CanHandleType(Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
        }

        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            var genericArguments = type.GetGenericArguments();
            var descriptorCreator = CreateDescriptorMethodInfo
                .MakeGenericMethod(genericArguments[0], genericArguments[1]);

            value = (ObjectDescriptor) descriptorCreator.Invoke(null, ArrayEx.Empty<object>());
            return true;
        }
        
        private static ObjectDescriptor CreateDescriptor<T, TV>()
        {
            return new ObjectDescriptor(typeof(KeyValuePair<T, TV>), new KeyValuePairAccessor<T, TV>(), GetProperties);
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