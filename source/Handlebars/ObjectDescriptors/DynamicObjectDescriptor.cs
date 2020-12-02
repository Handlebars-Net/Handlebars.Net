using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HandlebarsDotNet.Iterators;
using HandlebarsDotNet.MemberAccessors;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.ObjectDescriptors
{
    public sealed class DynamicObjectDescriptor : IObjectDescriptorProvider
    {
        private readonly ObjectDescriptorProvider _objectDescriptorProvider;
        private static readonly DynamicMemberAccessor DynamicMemberAccessor = new DynamicMemberAccessor();
        private static readonly Func<ObjectDescriptor, object, IEnumerable<object>> GetProperties = (descriptor, o) =>
        {
            var objectDescriptor = (ObjectDescriptor) descriptor.Dependencies[0];
            var properties = objectDescriptor!.GetProperties(objectDescriptor, o)
                .Cast<ChainSegment>();

            return ((IDynamicMetaObjectProvider) o)
                .GetMetaObject(Expression.Constant(o))
                .GetDynamicMemberNames()
                .Select(ChainSegment.Create)
                .Concat(properties);
        };
        private static readonly Type Type = typeof(IDynamicMetaObjectProvider);

        public DynamicObjectDescriptor(ObjectDescriptorProvider objectDescriptorProvider)
        {
            _objectDescriptorProvider = objectDescriptorProvider;
        }
        
        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            if (!Type.IsAssignableFrom(type))
            {
                value = ObjectDescriptor.Empty;
                return false;
            }
            
            if (!_objectDescriptorProvider.TryGetDescriptor(type, out var objectDescriptor))
            {
                value = ObjectDescriptor.Empty;
                return false;
            }
            
            var mergedMemberAccessor = new MergedMemberAccessor(objectDescriptor.MemberAccessor, DynamicMemberAccessor);
            value = new ObjectDescriptor(type, 
                mergedMemberAccessor, 
                GetProperties, 
                self => new DynamicObjectIterator(self),
                objectDescriptor
            );
            
            return true;
        }
    }
}