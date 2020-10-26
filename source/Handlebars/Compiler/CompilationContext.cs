using System.Linq.Expressions;
using Expressions.Shortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal sealed class CompilationContext
    {
        public CompilationContext(ICompiledHandlebarsConfiguration configuration)
        {
            Configuration = configuration;
            BindingContext = Expression.Parameter(typeof(BindingContext), "context");
            EncodedWriter = Expression.Parameter(typeof(EncodedTextWriter).MakeByRefType(), "writer");
            
            Args = new CompilationContextArgs(this);
        }

        public ICompiledHandlebarsConfiguration Configuration { get; }

        public ParameterExpression BindingContext { get; }
        
        public ParameterExpression EncodedWriter { get; }
        
        public CompilationContextArgs Args { get; }
        
        internal class CompilationContextArgs
        {
            public CompilationContextArgs(CompilationContext context)
            {
                BindingContext = new ExpressionContainer<BindingContext>(context.BindingContext);
                EncodedWriter = new ExpressionContainer<EncodedTextWriter>(context.EncodedWriter);
            }

            public ExpressionContainer<BindingContext> BindingContext { get; }
        
            public ExpressionContainer<EncodedTextWriter> EncodedWriter { get; }
        }
    }
}
