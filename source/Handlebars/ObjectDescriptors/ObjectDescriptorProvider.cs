using System;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.MemberAccessors;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal class ObjectDescriptorProvider : IObjectDescriptorProvider
    {
        private static readonly Type DynamicMetaObjectProviderType = typeof(IDynamicMetaObjectProvider);
        
        private readonly InternalHandlebarsConfiguration _configuration;
        private readonly RefLookup<Type, DeferredValue<string[]>> _membersCache = new RefLookup<Type, DeferredValue<string[]>>();

        public ObjectDescriptorProvider(InternalHandlebarsConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public bool CanHandleType(Type type)
        {
            return !DynamicMetaObjectProviderType.IsAssignableFrom(type) 
                   && type != typeof(string);
        }

        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            ref var members = ref _membersCache.GetOrAdd(type, DescriptorValueFactory);
            var membersValue = members.Value;
            
            value = new ObjectDescriptor(type)
            {
                GetProperties = o => membersValue,
                MemberAccessor = new ReflectionMemberAccessor(_configuration)
            };

            return true;
        }

        private static ref DeferredValue<string[]> DescriptorValueFactory(Type type, ref DeferredValue<string[]> deferredValue)
        {
            deferredValue.Factory = () =>
            {
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(o => o.CanRead && o.GetIndexParameters().Length == 0);
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                return properties.Cast<MemberInfo>().Concat(fields).Select(o => o.Name).ToArray();
            };

            return ref deferredValue;
        }
    }
}