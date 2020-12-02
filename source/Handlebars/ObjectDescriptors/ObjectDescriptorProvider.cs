using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.EqualityComparers;
using HandlebarsDotNet.Iterators;
using HandlebarsDotNet.MemberAccessors;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet.ObjectDescriptors
{
    public sealed class ObjectDescriptorProvider : IObjectDescriptorProvider
    {
        private static readonly Type StringType = typeof(string);
        
        private readonly LookupSlim<Type, DeferredValue<Type, ChainSegment[]>, ReferenceEqualityComparer<Type>> _membersCache = new LookupSlim<Type, DeferredValue<Type, ChainSegment[]>, ReferenceEqualityComparer<Type>>(new ReferenceEqualityComparer<Type>());
        private readonly ReflectionMemberAccessor _reflectionMemberAccessor;

        public ObjectDescriptorProvider(IReadOnlyList<IMemberAliasProvider> aliasProviders)
        {
            _reflectionMemberAccessor = new ReflectionMemberAccessor(aliasProviders);
        }
        
        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            if (type == StringType)
            {
                value = ObjectDescriptor.Empty;
                return false;
            }
            
            value = new ObjectDescriptor(
                type, 
                _reflectionMemberAccessor, 
                GetProperties, 
                self => new ObjectIterator(self), 
                dependencies: _membersCache
            );

            return true;
        }
        
        private static readonly Func<ObjectDescriptor, object, IEnumerable<object>> GetProperties = (descriptor, o) =>
        {
            var cache = (LookupSlim<Type, DeferredValue<Type, ChainSegment[]>, ReferenceEqualityComparer<Type>>) descriptor.Dependencies[0];
            return cache.GetOrAdd(descriptor.DescribedType, DescriptorValueFactory).Value;
        };
        
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
                        .Select(o => ChainSegment.Create(o))
                        .ToArray();
                });
            };
    }
}