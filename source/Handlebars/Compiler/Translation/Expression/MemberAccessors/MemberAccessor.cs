namespace HandlebarsDotNet.Compiler
{
    internal abstract class MemberAccessor
    {
        private readonly CompilationContext _compilationContext;

        protected MemberAccessor(CompilationContext compilationContext)
        {
            _compilationContext = compilationContext;
        }
        
        public abstract bool TryAccessMember(BindingContext context, object instance, string memberName, out object result);

        protected string ResolveMemberName(object instance, string memberName)
        {
            var resolver = _compilationContext.Configuration.ExpressionNameResolver;
            memberName = resolver != null
                ? resolver.ResolveExpressionName(instance, memberName)
                : memberName;
            
            memberName = memberName.Trim('[', ']'); // Ensure square brackets removed.
            return memberName;
        }
        
        protected static bool ContainsVariable(string segment)
        {
            return segment.StartsWith("@");
        }
    }
}