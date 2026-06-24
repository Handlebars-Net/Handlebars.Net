using System.Linq.Expressions;
using static HandlebarsDotNet.ExpressionShortcuts.ExpressionShortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal class StaticReplacer : HandlebarsExpressionVisitor
    {
        private CompilationContext CompilationContext { get; }

        public StaticReplacer(CompilationContext compilationContext)
        {
            CompilationContext = compilationContext;
        }
        
        protected override Expression VisitStaticExpression(StaticExpression stex)
        {
            var writer = CompilationContext.Args.EncodedWriter;
            var value = Arg(stex.Value);
            
            return writer.Call(o => o.Write(value, false));
        }
    }
}

