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
        private readonly Type _dynamicMetaObjectProviderType = typeof(IDynamicMetaObjectProvider);
        private readonly LookupSlim<Type, DeferredValue<Type, string[]>> _membersCache = new LookupSlim<Type, DeferredValue<Type, string[]>>();
        private readonly ReflectionMemberAccessor _reflectionMemberAccessor;

        public ObjectDescriptorProvider(InternalHandlebarsConfiguration configuration)
        {
            _reflectionMemberAccessor = new ReflectionMemberAccessor(configuration);
        }
        
        public bool CanHandleType(Type type)
        {
            return !_dynamicMetaObjectProviderType.IsAssignableFrom(type) && type != typeof(string);
        }

        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            value = new ObjectDescriptor(type, _reflectionMemberAccessor, (descriptor, o) =>
            {
                var cache = (LookupSlim<Type, DeferredValue<Type, string[]>>) descriptor.Dependencies[0];
                return cache.GetOrAdd(descriptor.DescribedType, DescriptorValueFactory).Value;
            }, dependencies: _membersCache);

            return true;
        }
        
        private static readonly Func<Type, DeferredValue<Type, string[]>> DescriptorValueFactory =
            key =>
            {
                return new DeferredValue<Type, string[]>(key, type =>
                {
                    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(o => o.CanRead && o.GetIndexParameters().Length == 0);
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                    return properties.Cast<MemberInfo>().Concat(fields).Select(o => o.Name).ToArray();
                });
            };
    }
}