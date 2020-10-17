using System;
using System.Collections;
using System.Reflection;
using HandlebarsDotNet.MemberAccessors;
using HandlebarsDotNet.MemberAccessors.EnumerableAccessors;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal class CollectionObjectDescriptor : IObjectDescriptorProvider
    {
        private readonly IObjectDescriptorProvider _objectDescriptorProvider;
        private static readonly Type Type = typeof(ICollection);

        public CollectionObjectDescriptor(IObjectDescriptorProvider objectDescriptorProvider)
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

            if (!_objectDescriptorProvider.TryGetDescriptor(type, out value))
            {
                value = ObjectDescriptor.Empty;
                return false;
            }
            
            var enumerableMemberAccessor = EnumerableMemberAccessor.Create(type);
            
            var mergedMemberAccessor = new MergedMemberAccessor(enumerableMemberAccessor, value.MemberAccessor);
            value = new ObjectDescriptor(value.DescribedType, mergedMemberAccessor, value.GetProperties, true);
            
            return true;
        }
    }
}