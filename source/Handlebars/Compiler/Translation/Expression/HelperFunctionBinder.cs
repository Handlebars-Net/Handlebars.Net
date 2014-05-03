using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Handlebars.Compiler
{
    internal class HelperFunctionBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, HandlebarsConfiguration configuration)
        {
            return new HelperFunctionBinder(configuration).Visit(expr);
        }

        private readonly HandlebarsConfiguration _configuration;

        private HelperFunctionBinder(HandlebarsConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            return Expression.Block(
                node.Expressions.Select(expr => Visit(expr)));
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            if(sex.Body is HelperExpression)
            {
                return Visit(sex.Body);
            }
            else
            {
                return sex;
            }
        }

        protected override Expression VisitBlockHelperExpression(BlockHelperExpression bhex)
        {
            return HandlebarsExpression.BlockHelper(
                bhex.HelperName,
                bhex.Arguments.Select(arg => Visit(arg)),
                Visit(bhex.Body));
        }

        protected override Expression VisitHelperExpression(HelperExpression hex)
        {
            if(_configuration.Helpers.ContainsKey(hex.HelperName))
            {
                return BindHelper(hex, _configuration.Helpers[hex.HelperName].Method);
            }
            else
            {
                return hex;
            }
        }

        private static Expression BindHelper(HelperExpression hex, MethodInfo method)
        {
            return Expression.Call(
                method,
                new Expression[] {
                    Expression.Property(
                        HandlebarsExpression.ContextAccessor(),
                        typeof(BindingContext).GetProperty("TextWriter")),
                    Expression.NewArrayInit(typeof(object), hex.Arguments)
                });
        }
    }
}

