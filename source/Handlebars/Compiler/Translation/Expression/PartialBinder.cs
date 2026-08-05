using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using Expressions.Shortcuts;
using HandlebarsDotNet.IO;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.Polyfills;
using static Expressions.Shortcuts.ExpressionShortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal class PartialBinder : HandlebarsExpressionVisitor
    {
        private static string SpecialPartialBlockName = "@partial-block";

        private static string ToPartialName(object value)
        {
            if (value is SafeString safe) return safe.Value;
            return (string) value;
        }

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
                        ? Arg<object?>(FunctionBuilder.Reduce(pex.Argument, CompilationContext, out _))
                        : bindingContext.Property(o => o.Value);

                    var partialTemplate = Arg(partialBlockTemplate);
                    bindingContext = bindingContext.Call(o => o.CreateChildContext(value, partialTemplate));
                }

                var partialNameObj = Arg<object>(pex.PartialName);
                var partialName = Call(() => ToPartialName(partialNameObj));
                var configuration = Arg(CompilationContext.Configuration);
                var isBlock = Arg(pex.IsBlock);
                var indent = Arg(pex.Indent);
                var templateDelegate = FunctionBuilder.Compile(
                    new []
                    {
                        Call(() =>
                            InvokePartialWithFallback(partialName, bindingContext, writer, (ICompiledHandlebarsConfiguration) configuration, isBlock, indent) // NOSONAR S1944 — ExpressionShortcuts operator; not a runtime hierarchy cast
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
                        ? Arg<object?>(FunctionBuilder.Reduce(pex.Argument, CompilationContext, out _))
                        : bindingContext.Property(o => o.Value);

                    var partialTemplate = Arg(partialBlockTemplate);
                    bindingContext = bindingContext.Call(o => o.CreateChildContext(value, partialTemplate));
                }

                var partialNameObj = Arg<object>(pex.PartialName);
                var partialName = Call(() => ToPartialName(partialNameObj));
                var configuration = Arg(CompilationContext.Configuration);
                var isBlock = Arg(pex.IsBlock);
                var indent = Arg(pex.Indent);

                return Call(() =>
                    InvokePartialWithFallback(partialName, bindingContext, writer, (ICompiledHandlebarsConfiguration) configuration, isBlock, indent)
                );
            }
        }

        private static void InvokePartialWithFallback(
            string partialName,
            BindingContext context,
            EncodedTextWriter writer,
            ICompiledHandlebarsConfiguration configuration,
            bool block,
            string? indent)
        {
            partialName = ChainSegment.Create(partialName).TrimmedValue;
            if (InvokePartial(partialName, context, writer, configuration, block, indent)) return;
            if (context.PartialBlockTemplate == null)
            {
                if (configuration.MissingPartialTemplateHandler == null)
                    throw new HandlebarsRuntimeException($"Referenced partial name '{partialName}' could not be resolved. If you registered the partial on the static Handlebars class, make sure to also compile and render using the same static class (or register the partial on the IHandlebars instance you are using to compile).");
                
                configuration.MissingPartialTemplateHandler.Handle(configuration, partialName, writer);
                return;
            }

            if (!string.IsNullOrEmpty(indent))
            {
                using var innerWriter = ReusableStringWriter.Get(writer.UnderlyingWriter.FormatProvider);
                using var textWriter = new EncodedTextWriter(innerWriter, configuration.TextEncoder, FormatterProvider.Current, true);
                context.PartialBlockTemplate(textWriter, context);
                WriteWithIndent(writer, innerWriter.ToString(), indent);
            }
            else
            {
                context.PartialBlockTemplate(writer, context);
            }
        }

        /// <summary>
        /// Writes <paramref name="content"/> to <paramref name="writer"/>, prepending <paramref name="indent"/>
        /// to every non-empty line.  All lines including the first receive the indent because the
        /// WhitespaceRemover already stripped the leading whitespace from the static token that preceded
        /// the partial tag.  An empty trailing segment after the last newline does not receive an indent.
        /// Newlines are normalised to <c>\n</c> so that output is consistent across platforms regardless
        /// of whether the partial source was checked out with <c>\r\n</c> line endings.
        /// </summary>
        private static void WriteWithIndent(EncodedTextWriter writer, string? content, string? indent)
        {
            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            // Normalise line endings to \n so Windows \r\n does not produce \r artifacts.
            var normalised = content.Replace("\r\n", "\n").Replace("\r", "\n");

            var pos = 0;
            while (pos < normalised.Length)
            {
                var newlinePos = normalised.IndexOf('\n', pos);
                if (newlinePos < 0)
                {
                    // No more newlines — write indent + rest and stop
                    writer.Write(indent, false);
                    writer.Write(normalised.Substring(pos), false);
                    break;
                }

                // Write indent + the segment up to and including the \n
                writer.Write(indent, false);
                writer.Write(normalised.Substring(pos, newlinePos - pos + 1), false);
                pos = newlinePos + 1;
            }
        }

        private static bool InvokePartial(
            string partialName,
            BindingContext context,
            EncodedTextWriter writer,
            ICompiledHandlebarsConfiguration configuration,
            bool block,
            string? indent)
        {
            if (partialName.Equals(SpecialPartialBlockName))
            {
                var partialBlockTemplate = context.PartialBlockTemplate;

                // If we are a block, our contents are the fallback and SpecialPartialBlockName refers to our parent
                if (block)
                {
                    partialBlockTemplate = context.ParentContext!.PartialBlockTemplate;
                }

                if (partialBlockTemplate == null)
                {
                    return false;
                }

                try
                {
                    context.PartialBlockTemplate = context.ParentContext!.PartialBlockTemplate;
                    if (!string.IsNullOrEmpty(indent))
                    {
                        using var innerWriter = ReusableStringWriter.Get(writer.UnderlyingWriter.FormatProvider);
                        using var textWriter = new EncodedTextWriter(innerWriter, configuration.TextEncoder, FormatterProvider.Current, true);
                        partialBlockTemplate(textWriter, context);
                        WriteWithIndent(writer, innerWriter.ToString(), indent);
                    }
                    else
                    {
                        partialBlockTemplate(writer, context);
                    }
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
                    if (!string.IsNullOrEmpty(indent))
                    {
                        using var innerWriter = ReusableStringWriter.Get(writer.UnderlyingWriter.FormatProvider);
                        using var textWriter = new EncodedTextWriter(innerWriter, configuration.TextEncoder, FormatterProvider.Current, true);
                        partial(textWriter, context);
                        WriteWithIndent(writer, innerWriter.ToString(), indent);
                    }
                    else
                    {
                        partial(writer, context);
                    }
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
                    || !configuration.PartialTemplateResolver.TryRegisterPartial(handlebars, partialName, (string?) context.Extensions.Optional("templatePath")))
                {
                    // Template not found.
                    return false;
                }
            }

            IncreaseDepth();
            try
            {
                if (!string.IsNullOrEmpty(indent))
                {
                    using var innerWriter = ReusableStringWriter.Get(writer.UnderlyingWriter.FormatProvider);
                    using (var encodedInner = new EncodedTextWriter(innerWriter, configuration.TextEncoder, FormatterProvider.Current, true))
                    {
                        using var textWriter = encodedInner.CreateWrapper();
                        configuration.RegisteredTemplates[partialName]!(textWriter, context);
                    }
                    WriteWithIndent(writer, innerWriter.ToString(), indent);
                }
                else
                {
                    using var textWriter = writer.CreateWrapper();
                    configuration.RegisteredTemplates[partialName]!(textWriter, context);
                }
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
