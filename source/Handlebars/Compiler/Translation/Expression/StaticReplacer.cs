using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HandlebarsDotNet.Compiler
{
    internal class StaticReplacer : HandlebarsExpressionVisitor
    {
        public static Expression Replace(Expression expr, CompilationContext context)
        {
            return new StaticReplacer(context).Visit(expr);
        }

        private StaticReplacer(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitStaticExpression(StaticExpression stex)
        {
            var context = ExpressionShortcuts.Arg<BindingContext>(CompilationContext.BindingContext);
            var writer = context.Property(o => o.TextWriter);
            return writer.Call(o => o.Write(stex.Value, false));
        }
    }
}

