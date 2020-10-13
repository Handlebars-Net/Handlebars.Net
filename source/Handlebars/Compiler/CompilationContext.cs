using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal sealed class CompilationContext
    {
        public CompilationContext(ICompiledHandlebarsConfiguration configuration)
        {
            Configuration = configuration;
            BindingContext = Expression.Variable(typeof(BindingContext), "context");
        }

        public ICompiledHandlebarsConfiguration Configuration { get; }

        public ParameterExpression BindingContext { get; }
    }
}
