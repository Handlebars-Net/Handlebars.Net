using System.Linq.Expressions;
using static Expressions.Shortcuts.ExpressionShortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal class BoolishConverter : HandlebarsExpressionVisitor
    {
        private readonly CompilationContext _compilationContext;

        public BoolishConverter(CompilationContext compilationContext)
        {
            _compilationContext = compilationContext;
        }
        
        protected override Expression VisitBoolishExpression(BoolishExpression bex)
        {
            var condition = Visit(bex.Condition);
            condition = FunctionBuilder.Reduce(condition, _compilationContext, out _);
            var @object = Arg<object>(condition);
            return Call(() => HandlebarsUtils.IsTruthyOrNonEmpty(@object));
        }
    }
}

