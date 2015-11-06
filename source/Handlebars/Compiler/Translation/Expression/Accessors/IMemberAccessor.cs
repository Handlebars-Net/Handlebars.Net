namespace HandlebarsDotNet.Compiler.Translation.Expression.Accessors
{
    internal interface IMemberAccessor
    {
        /// <summary>
        /// Determines if the memberName passed later should be the memberName or the resolvedMemberName.
        /// </summary>
        bool RequiresResolvedMemberName { get; }

        /// <summary>
        /// Determines if a member can be accessed using the current accessor.
        /// </summary>
        /// <param name="instance">Instance of the object to access.</param>
        /// <param name="memberName">Member of the instance to access.</param>
        /// <returns></returns>
        bool CanHandle(object instance, string memberName);

        /// <summary>
        /// Accesses the member and retrieves the result.
        /// </summary>
        /// <param name="instance">Instance of the object to access.</param>
        /// <param name="memberName">Member of the instance to access.</param>
        /// <returns></returns>
        object AccessMember(object instance, string memberName);
    }
}
