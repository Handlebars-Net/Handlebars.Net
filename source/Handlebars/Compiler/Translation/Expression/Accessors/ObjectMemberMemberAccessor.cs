using System;
using System.Collections.Generic;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors.Members;

namespace HandlebarsDotNet.Compiler.Translation.Expression.Accessors
{
    internal class ObjectMemberMemberAccessor : IMemberAccessor
    {
        //internal static readonly IDictionary<ObjectMemberKey, Func<object, object>> Cache =
        //    new Dictionary<ObjectMemberKey, Func<object, object>>(new ObjectMemberKeyEqualityComparer());

        /// <summary>
        /// Determines if the memberName passed later should be the memberName or the resolvedMemberName.
        /// </summary>
        public bool RequiresResolvedMemberName { get { return true; } }

        public static Func<object, object> GetMemberAccessor(Type instanceType, string memberName)
        {
            //Func<object, object> accessor;
            //var key = new ObjectMemberKey(instanceType, memberName);

            //if (!Cache.TryGetValue(key, out accessor))
            //{
            //    accessor = ObjectMemberExpressionBuilder.BuildMemberGetterFunc(instanceType, memberName);
            //    Cache.AddOrUpdate(key, accessor);
            //}

            var accessor = ObjectMemberExpressionBuilder.BuildMemberGetterFunc(instanceType, memberName);

            return accessor;
        }

        /// <summary>
        /// Determines if a member can be accessed using the current accessor.
        /// </summary>
        /// <param name="instance">Instance of the object to access.</param>
        /// <param name="memberName">Member of the instance to access.</param>
        /// <returns></returns>
        /// <remarks>
        /// This accessor should be used in last resort.
        /// </remarks>
        public bool CanHandle(object instance, string memberName)
        {
            var canHandle = instance != null;
            return canHandle;
        }

        /// <summary>
        /// Accesses the member and retrieves the result.
        /// </summary>
        /// <param name="instance">Instance of the object to access.</param>
        /// <param name="memberName">Member of the instance to access.</param>
        /// <returns></returns>
        public object AccessMember(object instance, string memberName)
        {
            var instanceType = instance.GetType();
            var accessor = GetMemberAccessor(instanceType, memberName);

            var value = accessor == null ? new UndefinedBindingResult() : accessor.Invoke(instance);
            return value;
        }
    }
}
