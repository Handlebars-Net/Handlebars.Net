using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors.Members;

namespace HandlebarsDotNet.Compiler.Translation.Expression.Accessors
{
    public class ObjectMemberMemberAccessor : IMemberAccessor
    {
        private static IDictionary<ObjectMemberKey, Func<object, object>> Cache = new Dictionary<ObjectMemberKey, Func<object, object>>();

        /// <summary>
        /// Determines if the memberName passed later should be the memberName or the resolvedMemberName.
        /// </summary>
        public bool RequiresResolvedMemberName { get { return true; } }

        public static Func<object, object> GetMemberAccessor(Type instanceType, string memberName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines if a member can be accessed using the current accessor.
        /// </summary>
        /// <param name="instance">Instance of the object to access.</param>
        /// <returns></returns>
        /// <remarks>
        /// This accessor should be used in last resort.
        /// </remarks>
        public bool CanHandle(object instance)
        {
            return true;
        }

        /// <summary>
        /// Accesses the member and retrieves the result.
        /// </summary>
        /// <param name="instance">Instance of the object to access.</param>
        /// <param name="memberName">Member of the instance to access.</param>
        /// <returns></returns>
        public object AccessMember(object instance, string memberName)
        {
            throw new NotImplementedException();
        }
    }
}
