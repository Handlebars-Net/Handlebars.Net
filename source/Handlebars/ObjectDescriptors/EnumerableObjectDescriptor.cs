using System;
using System.Collections;
using System.Reflection;
using HandlebarsDotNet.MemberAccessors;
using HandlebarsDotNet.MemberAccessors.EnumerableAccessors;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal class EnumerableObjectDescriptor : IObjectDescriptorProvider
    {
        private readonly IObjectDescriptorProvider _descriptorProvider;
        private static readonly Type Type = typeof(IEnumerable);
        private static readonly Type StringType = typeof(string);

        public EnumerableObjectDescriptor(IObjectDescriptorProvider descriptorProvider)
        {
            _descriptorProvider = descriptorProvider;
        }
        
        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            if (!(type != StringType && Type.IsAssignableFrom(type)))
            {
                value = ObjectDescriptor.Empty;
                return false;
            }
            
            if (!_descriptorProvider.TryGetDescriptor(type, out value))
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