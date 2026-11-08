using System.Linq.Expressions;

namespace HandlebarsDotNet.ExpressionShortcuts
{
    internal sealed class ReplaceParameterVisitor(ParameterExpression parameter, Expression argument)
        : UnpackExpressionContainerVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return ReferenceEquals(node, parameter)
                ? argument
                : base.VisitParameter(node);
        }
    }
}