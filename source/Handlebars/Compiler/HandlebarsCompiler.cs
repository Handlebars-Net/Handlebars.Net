using System;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Compiler.Lexer;
using HandlebarsDotNet.IO;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Compiler
{
    public delegate void TemplateDelegate(in EncodedTextWriter writer, BindingContext context);
    
    internal static class HandlebarsCompiler
    {
        public static TemplateDelegate Compile(ExtendedStringReader source, CompilationContext compilationContext)
        {
            var configuration = compilationContext.Configuration;
            var createdFeatures = configuration.Features;
            for (var index = 0; index < createdFeatures.Count; index++)
            {
                createdFeatures[index].OnCompiling(configuration);
            }
            
            var tokens = Tokenizer.Tokenize(source).ToArray();
            var expressions = ExpressionBuilder.ConvertTokensToExpressions(tokens, configuration);
            var action = FunctionBuilder.Compile(expressions, compilationContext);
            
            for (var index = 0; index < createdFeatures.Count; index++)
            {
                createdFeatures[index].CompilationCompleted();
            }

            return action;
        }

        internal static TemplateDelegate CompileView(ViewReaderFactory readerFactoryFactory, string templatePath, CompilationContext compilationContext)
        {
            var configuration = compilationContext.Configuration;
            IEnumerable<object> tokens;
            using (var sr = readerFactoryFactory(configuration, templatePath))
            {
                using (var reader = new ExtendedStringReader(sr))
                {
                    tokens = Tokenizer.Tokenize(reader).ToArray();
                }
            }

            var layoutToken = tokens.OfType<LayoutToken>().SingleOrDefault();
            
            var expressions = ExpressionBuilder.ConvertTokensToExpressions(tokens, configuration);
            var compiledView = FunctionBuilder.Compile(expressions, compilationContext);
            if (layoutToken == null) return compiledView;

            var fs = configuration.FileSystem;
            var layoutPath = fs.Closest(templatePath, layoutToken.Value + ".hbs");
            if (layoutPath == null)
                throw new InvalidOperationException($"Cannot find layout '{layoutToken.Value}' for template '{templatePath}'");

            var compiledLayout = CompileView(readerFactoryFactory, layoutPath, new CompilationContext(compilationContext));
            
            return (in EncodedTextWriter writer, BindingContext context) =>
            {
                var config = context.Configuration;
                using var bindingContext = BindingContext.Create(config, null);
                foreach (var pair in context.ContextDataObject)
                {
                    switch (pair.Key.WellKnownVariable)
                    {
                        case WellKnownVariable.Parent:
                        case WellKnownVariable.Root:
                            continue;
                    }
                    
                    bindingContext.ContextDataObject[pair.Key] = pair.Value;
                }
                
                using var innerWriter = ReusableStringWriter.Get(config.FormatProvider);
                using var textWriter = new EncodedTextWriter(innerWriter, config.TextEncoder, FormatterProvider.Current, true);
                compiledView(textWriter, context);
                var inner = innerWriter.ToString();

                var viewModel = new LayoutViewModel(inner, context.Value);
                bindingContext.Value = viewModel;

                compiledLayout(writer, bindingContext);
            };
        }
    }
}

