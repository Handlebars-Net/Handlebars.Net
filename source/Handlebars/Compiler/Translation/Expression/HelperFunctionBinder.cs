using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Handlebars.Compiler
{
    internal class HelperFunctionBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new HelperFunctionBinder(context).Visit(expr);
        }

        private readonly CompilationContext _context;

        private HelperFunctionBinder(CompilationContext context)
        {
            _context = context;
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            return Expression.Block(
                node.Variables,
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
            if (_context.Configuration.Helpers.ContainsKey(hex.HelperName))
            {
                return Expression.Call(
                    _context.Configuration.Helpers[hex.HelperName].Method,
                    new Expression[] {
                        Expression.Property(
                            _context.BindingContext,
                            typeof(BindingContext).GetProperty("TextWriter")),
                        Expression.NewArrayInit(typeof(object), hex.Arguments)
                    });
            }
            else
            {
                return hex;
            }
        }
    }
}

