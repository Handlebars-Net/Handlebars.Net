using System;
using System.IO;
using System.Linq;
using Handlebars.Compiler.Lexer;

namespace Handlebars.Compiler
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
    }
}

