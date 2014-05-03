using System;
using System.IO;
using Handlebars.Compiler.Lexer;

namespace Handlebars.Compiler
{
    internal class HandlebarsCompiler
    {
        private Tokenizer _tokenizer;
        private FunctionBuilder _builder;

        public HandlebarsCompiler(HandlebarsConfiguration configuration)
        {
            _tokenizer = new Tokenizer(configuration);
            _builder = new FunctionBuilder(configuration);
        }

        public Action<TextWriter, object> Compile(TextReader source)
        {
            var tokens = _tokenizer.Tokenize(source);
            return _builder.Compile(tokens);
        }
    }
}

