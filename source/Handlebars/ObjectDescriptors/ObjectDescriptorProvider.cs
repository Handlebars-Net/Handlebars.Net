using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.MemberAccessors;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal class ObjectDescriptorProvider : IObjectDescriptorProvider
    {
        private static readonly Type StringType = typeof(string);
        private static readonly DynamicObjectDescriptor DynamicObjectDescriptor = new DynamicObjectDescriptor();
        
        private readonly Type _dynamicMetaObjectProviderType = typeof(IDynamicMetaObjectProvider);
        private readonly LookupSlim<Type, DeferredValue<Type, ChainSegment[]>> _membersCache = new LookupSlim<Type, DeferredValue<Type, ChainSegment[]>>();
        private readonly ReflectionMemberAccessor _reflectionMemberAccessor;

        public ObjectDescriptorProvider(ICompiledHandlebarsConfiguration configuration)
        {
            _reflectionMemberAccessor = new ReflectionMemberAccessor(configuration);
        }
        
        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            if (type == StringType)
            {
                value = ObjectDescriptor.Empty;
                return false;
            }

            if (_dynamicMetaObjectProviderType.IsAssignableFrom(type))
            {
                if (DynamicObjectDescriptor.TryGetDescriptor(type, out var dynamicDescriptor))
                {
                    var mergedMemberAccessor = new MergedMemberAccessor(_reflectionMemberAccessor, dynamicDescriptor.MemberAccessor);
                    value = new ObjectDescriptor(type, 
                        mergedMemberAccessor, 
                        (descriptor, o) =>
                        {
                            var dynamicDescriptorGetProperties = dynamicDescriptor.GetProperties(descriptor, o)
                                .OfType<ChainSegment>();
                            
                            return GetProperties(descriptor, o)
                                .OfType<ChainSegment>()
                                .Concat(dynamicDescriptorGetProperties);
                        }, 
                        dependencies: _membersCache
                    );

                    return true;
                }
                
                value = ObjectDescriptor.Empty;
                return false;
            }
            
            value = new ObjectDescriptor(type, _reflectionMemberAccessor, GetProperties, dependencies: _membersCache);

            return true;
        }
        
        private static readonly Func<Type, DeferredValue<Type, ChainSegment[]>> DescriptorValueFactory =
            key =>
            {
                return new DeferredValue<Type, ChainSegment[]>(key, type =>
                {
                    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(o => o.CanRead && o.GetIndexParameters().Length == 0);
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                    return properties
                        .Cast<MemberInfo>()
                        .Concat(fields)
                        .Select(o => o.Name)
                        .Select(ChainSegment.Create)
                        .ToArray();
                });
            };

        private static readonly Func<ObjectDescriptor, object, IEnumerable<object>> GetProperties = (descriptor, o) =>
        {
            var cache = (LookupSlim<Type, DeferredValue<Type, ChainSegment[]>>) descriptor.Dependencies[0];
            return cache.GetOrAdd(descriptor.DescribedType, DescriptorValueFactory).Value;
        };
    }
}