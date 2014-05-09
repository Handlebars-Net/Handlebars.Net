using System;
using System.Linq;
using System.Linq.Expressions;

namespace Handlebars.Compiler
{
    internal class ContextBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr)
        {
            return new ContextBinder().Visit(expr);
        }

        private readonly ParameterExpression _contextParameter;

        private ContextBinder()
        {
            _contextParameter = Expression.Parameter(typeof(BindingContext));
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            return Expression.Invoke(Expression.Lambda(
                Expression.Block(
                    node.Expressions.Select(expr => Visit(expr))),
                _contextParameter), new Expression[] { _contextParameter });
        }
            
        protected override Expression VisitContextAccessorExpression(ContextAccessorExpression caex)
        {
            return _contextParameter;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return Expression.Call(
                Visit(node.Object),
                node.Method,
                node.Arguments.Select(arg => Visit(arg)));
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            return Expression.PropertyOrField(
                Visit(node.Expression),
                node.Member.Name);
        }
    }
}

