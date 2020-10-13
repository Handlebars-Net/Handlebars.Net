using System.Linq.Expressions;
using Expressions.Shortcuts;
using static Expressions.Shortcuts.ExpressionShortcuts;

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
            var context = Arg<BindingContext>(CompilationContext.BindingContext);
            var value = Arg(stex.Value);
            
            return context.Call(o => o.TextWriter.Write(value, false));
        }
    }
}

