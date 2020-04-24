using System;
using System.Linq.Expressions;
using Expressions.Shortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal class PartialBinder : HandlebarsExpressionVisitor
    {
        private static string SpecialPartialBlockName = "@partial-block";

        private CompilationContext CompilationContext { get; }
        
        public PartialBinder(CompilationContext compilationContext)
        {
            CompilationContext = compilationContext;
        }
        
        protected override Expression VisitBlockHelperExpression(BlockHelperExpression bhex) => bhex;

        protected override Expression VisitStatementExpression(StatementExpression sex) => sex.Body is PartialExpression ? Visit(sex.Body) : sex;

        protected override Expression VisitPartialExpression(PartialExpression pex)
        {
            var bindingContext = ExpressionShortcuts.Arg<BindingContext>(CompilationContext.BindingContext);
            var partialBlockTemplate = pex.Fallback != null 
                ? FunctionBuilder.CompileCore(new[] { pex.Fallback }, null, CompilationContext.Configuration) 
                : null;

            if (pex.Argument != null || partialBlockTemplate != null)
            {
                var value = ExpressionShortcuts.Arg<object>(FunctionBuilder.Reduce(pex.Argument, CompilationContext));
                var partialTemplate = ExpressionShortcuts.Arg(partialBlockTemplate);
                bindingContext = bindingContext.Call(o => o.CreateChildContext(value, partialTemplate));
            }

            var partialName = ExpressionShortcuts.Cast<string>(pex.PartialName);
            var configuration = ExpressionShortcuts.Arg(CompilationContext.Configuration);
            return ExpressionShortcuts.Call(() =>
                InvokePartialWithFallback(partialName, bindingContext, configuration)
            );
        }

        private static void InvokePartialWithFallback(
            string partialName,
            BindingContext context,
            HandlebarsConfiguration configuration)
        {
            if (InvokePartial(partialName, context, configuration)) return;
            if (context.PartialBlockTemplate == null)
            {
                if (configuration.MissingPartialTemplateHandler == null)
                    throw new HandlebarsRuntimeException($"Referenced partial name {partialName} could not be resolved");
                
                configuration.MissingPartialTemplateHandler.Handle(configuration, partialName, context.TextWriter);
                return;
            }

            context.PartialBlockTemplate(context.TextWriter, context);
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
            if (!configuration.RegisteredTemplates.ContainsKey(partialName))
            {
                if (configuration.PartialTemplateResolver == null 
                    || !configuration.PartialTemplateResolver.TryRegisterPartial(Handlebars.Create(configuration), partialName, context.TemplatePath))
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
                throw new HandlebarsRuntimeException($"Runtime error while rendering partial '{partialName}', see inner exception for more information", exception);
            }
        }
    }
}
