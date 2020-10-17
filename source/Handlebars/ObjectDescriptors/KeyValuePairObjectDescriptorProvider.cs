using System;
using System.Collections.Generic;
using System.Reflection;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.MemberAccessors;
using HandlebarsDotNet.Polyfills;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal sealed class KeyValuePairObjectDescriptorProvider : IObjectDescriptorProvider
    {
        private static readonly string[] Properties = { "key", "value" };
        private static readonly MethodInfo CreateDescriptorMethodInfo = typeof(KeyValuePairObjectDescriptorProvider).GetMethod(nameof(CreateDescriptor), BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly Func<ObjectDescriptor, object, IEnumerable<object>> GetProperties = (descriptor, o) => Properties;
        private static readonly Type Type = typeof(KeyValuePair<,>);
        
        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            if (!(type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == Type))
            {
                value = ObjectDescriptor.Empty;
                return false;
            }
            
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
            public bool TryGetValue(object instance, ChainSegment memberName, out object value)
            {
                var keyValuePair = (KeyValuePair<T, TV>) instance;

                if (memberName.IsKey)
                {
                    value = keyValuePair.Key;
                    return true;
                }
                
                if (memberName.IsValue)
                {
                    value = keyValuePair.Value;
                    return true;
                }
                
                value = default(TV);
                return false;
            }
        }
    }
}