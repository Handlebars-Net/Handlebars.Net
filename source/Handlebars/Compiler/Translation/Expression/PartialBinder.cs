using System;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
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
            Expression bindingContext = CompilationContext.BindingContext;
            if (pex.Argument != null)
            {
                bindingContext = Expression.Call(
                    bindingContext,
                    typeof(BindingContext).GetMethod("CreateChildContext"),
                    pex.Argument);
            }
            return Expression.Call(
                new Action<string, BindingContext, HandlebarsConfiguration>(InvokePartial).Method,
                Expression.Constant(pex.PartialName),
                bindingContext,
                Expression.Constant(CompilationContext.Configuration));
        }

        private static void InvokePartial(
            string partialName,
            BindingContext context,
            HandlebarsConfiguration configuration)
        {
            if (configuration.RegisteredTemplates.ContainsKey(partialName) == false)
            {
                if (configuration.FileSystem != null && context.TemplatePath != null)
                {
                    var partialPath = configuration.FileSystem.Closest(context.TemplatePath,
                        "partials/" + partialName + ".hbs");
                    if (partialPath != null)
                    {
                        var compiled = Handlebars.Create(configuration)
                            .CompileView(partialPath);
                        configuration.RegisteredTemplates.Add(partialName, (writer, o) =>
                        {
                            writer.Write(compiled(o));
                        });
                    }
                }
                else
                {
                    throw new HandlebarsRuntimeException(
                        string.Format("Referenced partial name {0} could not be resolved", partialName));
                }
            }
            configuration.RegisteredTemplates[partialName](context.TextWriter, context);
        }
    }
}

