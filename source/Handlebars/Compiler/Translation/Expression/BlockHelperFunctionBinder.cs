using System;
using System.Linq;
using System.Linq.Expressions;

namespace Handlebars.Compiler
{
    internal class BlockHelperFunctionBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, HandlebarsConfiguration configuration)
        {
            return new BlockHelperFunctionBinder(configuration).Visit(expr);
        }

        private readonly HandlebarsConfiguration _configuration;

        private BlockHelperFunctionBinder(HandlebarsConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            if(sex.Body is BlockHelperExpression)
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
            var fb = new FunctionBuilder(_configuration);
            var action = fb.Compile(((BlockExpression)bhex.Body).Expressions);
            return Expression.Call(
                _configuration.BlockHelpers[bhex.HelperName].Method,
                new Expression[] {
                    Expression.Property(
                        HandlebarsExpression.ContextAccessor(),
                        typeof(BindingContext).GetProperty("TextWriter")),
                    Expression.Constant(action),
                    Expression.NewArrayInit(typeof(object), bhex.Arguments)
                });
        }
    }
}

