using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Handlebars.Compiler.Lexer
{
    internal class Tokenizer
    {
        private readonly HandlebarsConfiguration _configuration;

        private static Parser _wordParser = new WordParser ();
        private static LiteralParser _literalParser = new LiteralParser();
        //TODO: structure parser

        public Tokenizer(HandlebarsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<Token> Tokenize(TextReader source)
        {
            try
            {
                return Parse(source);
            }
            catch(Exception ex)
            {
                throw new HandlebarsParserException("An unhandled exception occurred while trying to compile the template", ex);
            }
        }

        private IEnumerable<Token> Parse(TextReader source)
        {
            bool inExpression = false;
            var buffer = new StringBuilder ();
            while(true)
            {
                var node = source.Read();
                if(node == -1) 
                {
                    if(buffer.Length > 0)
                    {
                        if(inExpression)
                        {
                            throw new InvalidOperationException("Reached end of template before expression was closed");
                        }
                        else
                        {
                            yield return Token.Static (buffer.ToString ());
                        }
                    }
                    break;
                }
                if(inExpression)
                {
                    Token token = null;
                    token = token ?? _wordParser.Parse (source);
                    token = token ?? _literalParser.Parse (source);

                    if(token != null)
                    {
                        yield return token;
                    }

                    if((char)node == '}' && (char)source.Peek() == '}')
                    {
                        yield return Token.EndExpression ();
                        source.Read();
                        inExpression = false;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    if((char)node == '{' && (char)source.Peek() == '{')
                    {
                        yield return Token.Static (buffer.ToString ());
                        yield return Token.StartExpression ();
                        buffer = new StringBuilder();
                        inExpression = true;
                    }
                    else
                    {
                        buffer.Append((char)node);
                    }
                }
            }
        }
    }
}

