using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using HandlebarsDotNet.MemberAccessors;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal class DynamicObjectDescriptor : IObjectDescriptorProvider
    {
        public bool CanHandleType(Type type)
        {
            return typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type);
        }

        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            value = new ObjectDescriptor(type)
            {
                GetProperties = o => ((IDynamicMetaObjectProvider) o).GetMetaObject(Expression.Constant(o)).GetDynamicMemberNames(),
                MemberAccessor = new DynamicMemberAccessor()
            };

            return true;
        }
    }
}