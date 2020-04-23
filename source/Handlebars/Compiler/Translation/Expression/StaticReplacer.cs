using System.Linq.Expressions;
using Expressions.Shortcuts;

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
            var context = ExpressionShortcuts.Arg<BindingContext>(CompilationContext.BindingContext);
            var value = ExpressionShortcuts.Arg(stex.Value);
            
            return context.Call(o => o.TextWriter.Write(value, false));
        }
    }
}

