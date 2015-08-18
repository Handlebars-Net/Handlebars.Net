using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HandlebarsDotNet.Compiler.Lexer;

namespace HandlebarsDotNet.Compiler
{
    internal class HandlebarsCompiler
    {
        private Tokenizer _tokenizer;
        private FunctionBuilder _functionBuilder;
        private ExpressionBuilder _expressionBuilder;

        public HandlebarsCompiler(HandlebarsConfiguration configuration)
        {
            _tokenizer = new Tokenizer(configuration);
            _expressionBuilder = new ExpressionBuilder(configuration);
            _functionBuilder = new FunctionBuilder(configuration);
        }

        public Action<TextWriter, object> Compile(TextReader source)
        {
            var tokens = _tokenizer.Tokenize(source).ToList();
            var expressions = _expressionBuilder.ConvertTokensToExpressions(tokens);
            return _functionBuilder.Compile(expressions);
        }

        internal Action<TextWriter, object> CompileView(string templatePath, ViewEngineFileSystem fs)
        {
            var template = fs.GetFileContent(templatePath);
            if (template == null) throw new InvalidOperationException("Cannot find template at '" + templatePath + "'");
            IEnumerable<object> tokens = null;
            using (var sr = new StringReader(template))
            {
                tokens = _tokenizer.Tokenize(sr).ToList();
            }
            var layoutToken = tokens.OfType<LayoutToken>().SingleOrDefault();

            var expressions = _expressionBuilder.ConvertTokensToExpressions(tokens);
            var compiledView = _functionBuilder.Compile(expressions);
            if (layoutToken == null) return compiledView;

            var layoutPath = fs.Closest(templatePath, layoutToken.Value + ".hbs");
            if (layoutPath == null) throw new InvalidOperationException("Cannot find layout '" + layoutPath + "' for template '" + templatePath + "'");

            var compiledLayout = CompileView(layoutPath, fs);

            return (tw, vm) =>
            {
                var sb = new StringBuilder();
                using (var innerWriter = new StringWriter(sb))
                {
                    compiledView(innerWriter, vm);
                }
                var inner = sb.ToString();
                compiledLayout(tw, new {body = inner});
            };
        }

    
        
    }
}

