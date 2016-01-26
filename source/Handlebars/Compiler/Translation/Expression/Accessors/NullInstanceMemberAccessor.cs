namespace HandlebarsDotNet.Compiler.Translation.Expression.Accessors
{
    /// <summary>
    /// Handles null instances.
    /// </summary>
    internal sealed class NullInstanceMemberAccessor: IMemberAccessor
    {
        public bool RequiresResolvedMemberName { get; } = false;

        public bool CanHandle(object instance, string memberName)
        {
            var canHandle = instance == null;
            return canHandle;
        }

        public object AccessMember(object instance, string memberName)
        {
            return new UndefinedBindingResult();
        }
    }
}
