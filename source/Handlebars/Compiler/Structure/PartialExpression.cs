using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class PartialExpression : HandlebarsExpression
    {
        public PartialExpression(Expression partialName, Expression argument, Expression fallback)
        {
            PartialName = partialName;
            Argument = argument;
            Fallback = fallback;
        }

        public override ExpressionType NodeType => (ExpressionType)HandlebarsExpressionType.PartialExpression;

        public Expression PartialName { get; }

        public Expression Argument { get; }

        public Expression Fallback { get; }
    }
}

