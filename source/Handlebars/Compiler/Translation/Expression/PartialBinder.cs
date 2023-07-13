using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Expressions.Shortcuts;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.Polyfills;
using static Expressions.Shortcuts.ExpressionShortcuts;

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
            IReadOnlyList<DecoratorDefinition> decorators = ArrayEx.Empty<DecoratorDefinition>();
            var partialBlockTemplate = pex.Fallback != null 
                ? FunctionBuilder.Compile(new[] { pex.Fallback }, CompilationContext, out decorators) 
                : null;
            
            if (decorators.Count > 0)
            {
                var bindingContext = CompilationContext.Args.BindingContext;
                var writer = CompilationContext.Args.EncodedWriter;
            
                var parentContext = bindingContext;
                if (pex.Argument != null || partialBlockTemplate != null)
                {
                    var value = pex.Argument != null
                        ? Arg<object>(FunctionBuilder.Reduce(pex.Argument, CompilationContext, out _))
                        : bindingContext.Property(o => o.Value);
                
                    var partialTemplate = Arg(partialBlockTemplate);
                    bindingContext = bindingContext.Call(o => o.CreateChildContext(value, partialTemplate));
                }

                var partialName = Cast<string>(pex.PartialName);
                var configuration = Arg(CompilationContext.Configuration);
                var templateDelegate = FunctionBuilder.Compile(
                    new []
                    {
                        Call(() =>
                            InvokePartialWithFallback(partialName, bindingContext, writer, (ICompiledHandlebarsConfiguration) configuration)
                        ).Expression
                    }, 
                    CompilationContext, 
                    out _
                );

                var decorator = decorators.Compile(CompilationContext);
                return Call(() => decorator.Invoke(writer, parentContext, templateDelegate))
                    .Call(f => f.Invoke(writer, parentContext));
            }
            else
            {
                var bindingContext = CompilationContext.Args.BindingContext;
                var writer = CompilationContext.Args.EncodedWriter;
            
                if (pex.Argument != null || partialBlockTemplate != null)
                {
                    var value = pex.Argument != null
                        ? Arg<object>(FunctionBuilder.Reduce(pex.Argument, CompilationContext, out _))
                        : bindingContext.Property(o => o.Value);
                
                    var partialTemplate = Arg(partialBlockTemplate);
                    bindingContext = bindingContext.Call(o => o.CreateChildContext(value, partialTemplate));
                }

                var partialName = Cast<string>(pex.PartialName);
                var configuration = Arg(CompilationContext.Configuration);
                
                return Call(() =>
                    InvokePartialWithFallback(partialName, bindingContext, writer, (ICompiledHandlebarsConfiguration) configuration)
                );
            }
        }

        private static void InvokePartialWithFallback(
            string partialName,
            BindingContext context,
            EncodedTextWriter writer,
            ICompiledHandlebarsConfiguration configuration)
        {
            partialName = partialName != null ? ChainSegment.Create(partialName).TrimmedValue : null;
            if (InvokePartial(partialName, context, writer, configuration)) return;
            if (context.PartialBlockTemplate == null)
            {
                if (configuration.MissingPartialTemplateHandler == null)
                    throw new HandlebarsRuntimeException($"Referenced partial name {partialName} could not be resolved");
                
                configuration.MissingPartialTemplateHandler.Handle(configuration, partialName, writer);
                return;
            }

            context.PartialBlockTemplate(writer, context);
        }

        private static bool InvokePartial(
            string partialName,
            BindingContext context,
            EncodedTextWriter writer,
            ICompiledHandlebarsConfiguration configuration)
        {
            if (partialName.Equals(SpecialPartialBlockName))
            {
                if (context.PartialBlockTemplate == null)
                {
                    return false;
                }

                var partialBlockTemplate = context.PartialBlockTemplate;
                try
                {
                    context.PartialBlockTemplate = context.ParentContext.PartialBlockTemplate;
                    partialBlockTemplate(writer, context);
                }
                finally
                {
                    context.PartialBlockTemplate = partialBlockTemplate;
                }
                return true;
            }

            void IncreaseDepth()
            {
                if (++context.PartialDepth > configuration.PartialRecursionDepthLimit)
                    throw new HandlebarsRuntimeException($"Runtime error while rendering partial '{partialName}', exceeded recursion depth limit of {configuration.PartialRecursionDepthLimit}");
            }

            //if we have an inline partial, skip the file system and RegisteredTemplates collection
            if (context.InlinePartialTemplates.TryGetValue(partialName, out var partial))
            {
                IncreaseDepth();
                try
                {
                    partial(writer, context);
                }
                finally
                {
                    context.PartialDepth--;
                }
                return true;
            }
            
            // Partial is not found, so call the resolver and attempt to load it.
            if (!configuration.RegisteredTemplates.ContainsKey(partialName))
            {
                var handlebars = Handlebars.Create(configuration);
                if (configuration.PartialTemplateResolver == null 
                    || !configuration.PartialTemplateResolver.TryRegisterPartial(handlebars, partialName, (string) context.Extensions.Optional("templatePath")))
                {
                    // Template not found.
                    return false;
                }
            }

            IncreaseDepth();
            try
            {
                using var textWriter = writer.CreateWrapper();
                configuration.RegisteredTemplates[partialName](textWriter, context);
                return true;
            }
            catch (Exception exception)
            {
                throw new HandlebarsRuntimeException($"Runtime error while rendering partial '{partialName}', see inner exception for more information", exception);
            }
            finally
            {
                context.PartialDepth--;
            }
        }
    }
}
