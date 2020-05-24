using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using HandlebarsDotNet.MemberAccessors;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal class DynamicObjectDescriptor : IObjectDescriptorProvider
    {
        private static readonly DynamicMemberAccessor DynamicMemberAccessor = new DynamicMemberAccessor();
        private static readonly Func<ObjectDescriptor, object, IEnumerable<object>> GetProperties = (descriptor, o) => ((IDynamicMetaObjectProvider) o).GetMetaObject(Expression.Constant(o)).GetDynamicMemberNames();

        public bool CanHandleType(Type type)
        {
            return typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type);
        }

        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            value = new ObjectDescriptor(type, DynamicMemberAccessor, GetProperties);

            return true;
        }
    }
}