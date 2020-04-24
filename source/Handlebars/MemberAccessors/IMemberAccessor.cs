using System;

namespace HandlebarsDotNet.MemberAccessors
{
    /// <summary>
    /// Describes mechanism to access members of object
    /// </summary>
    public interface IMemberAccessor
    {
        /// <summary>
        /// Describes mechanism to access members of an object. Returns <see langword="true"/> if operation is successful and <paramref name="value"/> contains data, otherwise returns <see langword="false"/> 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="instanceType"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGetValue(object instance, Type instanceType, string memberName, out object value);
    }
}