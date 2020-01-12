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
                    typeof(BindingContext).GetMethod(nameof(BindingContext.CreateChildContext)),
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
                {
                    if (configuration.MissingPartialTemplateHandler != null)
                    {
                        configuration.MissingPartialTemplateHandler.Handle(configuration, partialName, context.TextWriter);
                        return;
                    }
                    else
                    {
                        throw new HandlebarsRuntimeException(string.Format("Referenced partial name {0} could not be resolved", partialName));
                    }
                }

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
            
            // Partial is not found, so call the resolver and attempt to load it.
            if (configuration.RegisteredTemplates.ContainsKey(partialName) == false)
            {
                if (configuration.PartialTemplateResolver == null 
                    || configuration.PartialTemplateResolver.TryRegisterPartial(Handlebars.Create(configuration), partialName, context.TemplatePath) == false)
                {
                    // Template not found.
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
