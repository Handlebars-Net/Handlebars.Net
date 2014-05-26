using System;
using System.Linq.Expressions;

namespace Handlebars.Compiler
{
    internal class PartialBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new PartialBinder(context).Visit(expr);
        }

        private readonly CompilationContext _context;

        private PartialBinder(CompilationContext context)
        {
            _context = context;
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            if(sex.Body is PartialExpression)
            {
                return Visit(sex.Body);
            }
            else
            {
                return sex;
            }
        }

        protected override Expression VisitPartialExpression(PartialExpression pex)
        {
            if(_context.Configuration.RegisteredTemplates.ContainsKey(pex.PartialName) == false)
            {
                throw new HandlebarsCompilerException("Referenced partial name could not be resolved");
            }
            return Expression.Call(
                new Action<string, BindingContext, HandlebarsConfiguration>(InvokePartial).Method,
                Expression.Constant(pex.PartialName),
                _context.BindingContext,
                Expression.Constant(_context.Configuration));
        }

        private static void InvokePartial(
            string partialName,
            BindingContext context,
            HandlebarsConfiguration configuration)
        {
            if(configuration.RegisteredTemplates.ContainsKey(partialName) == false)
            {
                throw new HandlebarsRuntimeException("Referenced partial name could not be resolved");
            }
            configuration.RegisteredTemplates[partialName](context.TextWriter, context.Value);
        }
    }
}

