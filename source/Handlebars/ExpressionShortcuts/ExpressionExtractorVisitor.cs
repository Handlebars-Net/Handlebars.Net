using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace HandlebarsDotNet.ExpressionShortcuts
{
    internal class ExpressionExtractorVisitor : ExpressionVisitor
    {
        [return: NotNullIfNotNull("node")]
        public override Expression? Visit(Expression? node)
        {
            if (node is LambdaExpression lambda)
            {
                return ExpressionUtils.ProcessCall(lambda.Body);
            }

            return base.Visit(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var dynamicInvoke = Expression.Lambda(node).Compile().DynamicInvoke();
            return ConvertToExpression(dynamicInvoke);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            switch (node.Expression)
            {
                case ConstantExpression constant:
                    var constantValue = constant.Value;
                    var value = constantValue!.GetType().GetField(node.Member.Name)?.GetValue(constantValue);
                    if (value is ExpressionContainer expressionContainer) return expressionContainer.Expression;
                    if (value?.GetType() == node.Type) return ConvertToExpression(value);
                    
                    return Visit(Expression.Convert(Expression.Constant(value), node.Type));

                default: 
                    return base.VisitMember(node);
            }
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.ConvertChecked:
                case ExpressionType.Convert:
                {
                    if (!typeof(ExpressionContainer).IsAssignableFrom(node.Operand.Type))
                        return node.Type == node.Operand.Type
                            ? node.Update(Visit(node.Operand))
                            : node;
                    
                    var operand = Visit(node.Operand);
                    if (operand.Type == typeof(void)) return operand;

                    if (typeof(ExpressionContainer).IsAssignableFrom(node.Type))
                    {
                        return operand;
                    }
                    
                    return operand.Type != node.Type 
                        ? Expression.Convert(operand, node.Type) 
                        : operand;
                }

                default:
                    return base.VisitUnary(node);
            }
        }

        private Expression ConvertToExpression(object? value)
        {
            if (value is ExpressionContainer expressionContainer) return expressionContainer.Expression;
            if (value is Expression expression) return Visit(expression);
            return Expression.Constant(value);
        }
    }
}