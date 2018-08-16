using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace HandlebarsDotNet.Compiler
{
    internal class PartialBinder : HandlebarsExpressionVisitor
    {
        private static string SpecialPartialBlockName = "@partial-block";

        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new PartialBinder(context).Visit(expr);
        }

        private PartialBinder(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitBlockHelperExpression(BlockHelperExpression bhex)
        {
            return bhex;
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

            var fb = new FunctionBuilder(CompilationContext.Configuration);
            var partialBlockTemplate = pex.Fallback == null ? null : fb.Compile(new[] {pex.Fallback}, null, null);

            if (pex.Argument != null || partialBlockTemplate != null)
            {
                bindingContext = Expression.Call(
                    bindingContext,
                    typeof(BindingContext).GetMethod("CreateChildContext"),
                    pex.Argument ?? Expression.Constant(null),
                    partialBlockTemplate ?? Expression.Constant(null, typeof(Action<TextWriter, object>)));
            }

            var partialInvocation = Expression.Call(
#if netstandard
                new Action<string, BindingContext, HandlebarsConfiguration>(InvokePartialWithFallback).GetMethodInfo(),
#else
                new Action<string, BindingContext, HandlebarsConfiguration>(InvokePartialWithFallback).Method,
#endif
                Expression.Convert(pex.PartialName, typeof(string)),
                bindingContext,
                Expression.Constant(CompilationContext.Configuration));

            return partialInvocation;
        }

        private static void InvokePartialWithFallback(
            string partialName,
            BindingContext context,
            HandlebarsConfiguration configuration)
        {
            if (!InvokePartial(partialName, context, configuration))
            {
                if (context.PartialBlockTemplate == null)
                    throw new HandlebarsRuntimeException(
                        string.Format("Referenced partial name {0} could not be resolved", partialName));

                context.PartialBlockTemplate(context.TextWriter, context);
            }
        }

        private static bool InvokePartial(
            string partialName,
            BindingContext context,
            HandlebarsConfiguration configuration)
        {
            if (partialName.Equals(SpecialPartialBlockName))
            {
                if (context.PartialBlockTemplate == null)
                {
                    return false;
                }

                context.PartialBlockTemplate(context.TextWriter, context);
                return true;
            }

            //if we have an inline partial, skip the file system and RegisteredTemplates collection
            if (context.InlinePartialTemplates.ContainsKey(partialName))
            {
                context.InlinePartialTemplates[partialName](context.TextWriter, context);
                return true;
            }

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
                    else
                    {
                        // Failed to find partial in filesystem
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            try
            {
                configuration.RegisteredTemplates[partialName](context.TextWriter, context);
                return true;
            }
            catch (Exception exception)
            {
                throw new HandlebarsRuntimeException(
                    $"Runtime error while rendering partial '{partialName}', see inner exception for more information",
                    exception);
            }
        }
    }
}
