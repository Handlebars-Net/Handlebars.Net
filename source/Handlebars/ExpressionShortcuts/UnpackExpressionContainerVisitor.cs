using System.Linq.Expressions;
using System.Reflection;

namespace HandlebarsDotNet.ExpressionShortcuts
{
    internal class UnpackExpressionContainerVisitor : ExpressionVisitor
    {
        public static readonly UnpackExpressionContainerVisitor Instance = new();

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node is
                {
                    Member: FieldInfo fieldInfo,
                    Expression: {} nextedExpression
                }
                && typeof(ExpressionContainer).IsAssignableFrom(fieldInfo.FieldType))
            {
                var nestedValue = ExtractFieldOrConstantValue(nextedExpression);
                if (nestedValue != null
                    && fieldInfo.GetValue(nestedValue) is ExpressionContainer expressionContainer)
                    return expressionContainer.Expression;
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            // Did somebody use the implicit operator to convert ExpressionContainer<T> to T?
            if (node.NodeType is ExpressionType.ConvertChecked or ExpressionType.Convert
                && typeof(ExpressionContainer).IsAssignableFrom(node.Operand.Type))
            {
                var operand = Visit(node.Operand);

                return operand.Type != node.Type
                    ? Expression.Convert(operand, node.Type)
                    : operand;
            }

            return base.VisitUnary(node);
        }

        // The C# compiler actually creates closure-objects that put values as fields on an anonymous object -> unwrap
        private static object? ExtractFieldOrConstantValue(Expression node)
        {
            switch (node)
            {
                case ConstantExpression constantExpression:
                    return constantExpression.Value;
                case MemberExpression
                {
                    Member: FieldInfo fieldInfo,
                    Expression: { } nestedExpression
                }:
                {
                    // In certain cases the compiler may use nested structures.
                    var nestedValue = ExtractFieldOrConstantValue(nestedExpression);
                    return nestedValue == null ? null : fieldInfo.GetValue(nestedValue);
                }
                default:
                    return null;
            }
        }
    }
}