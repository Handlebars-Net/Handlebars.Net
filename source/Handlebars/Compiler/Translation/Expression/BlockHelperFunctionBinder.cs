using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace Handlebars.Compiler
{
    internal class BlockHelperFunctionBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new BlockHelperFunctionBinder(context).Visit(expr);
        }

        private readonly CompilationContext _context;

        private BlockHelperFunctionBinder(CompilationContext context)
        {
            _context = context;
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
            var fb = new FunctionBuilder(_context.Configuration);
            var body = fb.Compile(((BlockExpression)bhex.Body).Expressions, _context.BindingContext);
            var helper = _context.Configuration.BlockHelpers[bhex.HelperName.Replace("#", "")];
            var arguments = new Expression[] {
                Expression.Property(
                    _context.BindingContext,
                    typeof(BindingContext).GetProperty("TextWriter")),
                body,
                Expression.Property(
                    _context.BindingContext,
                    typeof(BindingContext).GetProperty("Value")),
                Expression.NewArrayInit(typeof(object), bhex.Arguments)
            };
            if(helper.Target != null)
            {
                return Expression.Call(
                    Expression.Constant(helper.Target),
                    helper.Method,
                    arguments);
            }
            else
            {
                return Expression.Call(
                    helper.Method,
                    arguments);
            }
        }
    }
}

