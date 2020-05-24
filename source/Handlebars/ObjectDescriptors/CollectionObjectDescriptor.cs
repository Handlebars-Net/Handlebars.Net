using System;
using System.Collections;
using System.Reflection;
using HandlebarsDotNet.MemberAccessors;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal class CollectionObjectDescriptor : IObjectDescriptorProvider
    {
        private readonly ObjectDescriptorProvider _objectDescriptorProvider;

        public CollectionObjectDescriptor(ObjectDescriptorProvider objectDescriptorProvider)
        {
            _objectDescriptorProvider = objectDescriptorProvider;
        }
        
        public bool CanHandleType(Type type)
        {
            return typeof(ICollection).IsAssignableFrom(type) && _objectDescriptorProvider.CanHandleType(type);
        }

        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            if (!_objectDescriptorProvider.TryGetDescriptor(type, out value)) return false;
            
            var mergedMemberAccessor = new MergedMemberAccessor(new EnumerableMemberAccessor(), value.MemberAccessor);
            value = new ObjectDescriptor(
                value.DescribedType, 
                mergedMemberAccessor,
                value.GetProperties,
                true
            );
            
            return true;

        }
    }

    internal class MergedMemberAccessor : IMemberAccessor
    {
        private readonly IMemberAccessor[] _accessors;

        public MergedMemberAccessor(params IMemberAccessor[] accessors)
        {
            _accessors = accessors;
        }

        public bool TryGetValue(object instance, Type type, string memberName, out object value)
        {
            for (var index = 0; index < _accessors.Length; index++)
            {
                if (_accessors[index].TryGetValue(instance, type, memberName, out value)) return true;
            }

            value = default(object);
            return false;
        }
    }
}