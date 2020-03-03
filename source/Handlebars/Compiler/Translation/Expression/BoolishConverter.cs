using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class BoolishConverter : HandlebarsExpressionVisitor
    {
        public static Expression Convert(Expression expr, CompilationContext context)
        {
            return new BoolishConverter(context).Visit(expr);
        }

        private BoolishConverter(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitBoolishExpression(BoolishExpression bex)
        {
            return ExpressionShortcuts.Call(() => HandlebarsUtils.IsTruthyOrNonEmpty(Visit(bex.Condition)));
        }
    }
}

