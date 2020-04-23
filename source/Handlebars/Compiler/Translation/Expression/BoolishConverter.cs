using System.Linq.Expressions;
using Expressions.Shortcuts;

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
            condition = FunctionBuilder.Reduce(condition, _compilationContext);
            var @object = ExpressionShortcuts.Arg<object>(condition);
            return ExpressionShortcuts.Call(() => HandlebarsUtils.IsTruthyOrNonEmpty(@object));
        }
    }
}

