using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal sealed class CompilationContext
    {
        public CompilationContext(ICompiledHandlebarsConfiguration configuration)
        {
            Configuration = configuration;
            BindingContext = Expression.Parameter(typeof(BindingContext), "context");
            EncodedWriter = Expression.Parameter(typeof(EncodedTextWriter).MakeByRefType(), "writer");
        }

        public ICompiledHandlebarsConfiguration Configuration { get; }

        public ParameterExpression BindingContext { get; }
        
        public ParameterExpression EncodedWriter { get; }
    }
}
