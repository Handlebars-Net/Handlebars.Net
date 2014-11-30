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

        private PartialBinder(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            if (sex.Body is PartialExpression)
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
            return Expression.Call(
                new Action<string, BindingContext, HandlebarsConfiguration>(InvokePartial).Method,
                Expression.Constant(pex.PartialName),
                CompilationContext.BindingContext,
                Expression.Constant(CompilationContext.Configuration));
        }

        private static void InvokePartial(
            string partialName,
            BindingContext context,
            HandlebarsConfiguration configuration)
        {
            if (configuration.RegisteredTemplates.ContainsKey(partialName) == false)
            {
                throw new HandlebarsRuntimeException("Referenced partial name could not be resolved");
            }
            configuration.RegisteredTemplates[partialName](context.TextWriter, context);
        }
    }
}

