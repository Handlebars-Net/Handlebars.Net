using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal sealed class CompilationContext
    {
        public CompilationContext(InternalHandlebarsConfiguration configuration)
        {
            Configuration = configuration;
            BindingContext = Expression.Variable(typeof(BindingContext), "context");
        }

        public InternalHandlebarsConfiguration Configuration { get; }

        public ParameterExpression BindingContext { get; }
    }
}
