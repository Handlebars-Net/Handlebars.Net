using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class Tokenizer
    {
        private readonly HandlebarsConfiguration _configuration;

        private static readonly Parser WordParser = new WordParser();
        private static readonly Parser LiteralParser = new LiteralParser();
        private static readonly Parser CommentParser = new CommentParser();
        private static readonly Parser PartialParser = new PartialParser();
        private static readonly Parser BlockWordParser = new BlockWordParser();
        private static readonly Parser BlockParamsParser = new BlockParamsParser();
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
            catch (Exception ex)
            {
                throw new HandlebarsParserException("An unhandled exception occurred while trying to compile the template", ex);
            }
        }

        private IEnumerable<Token> Parse(TextReader source)
        {
            bool inExpression = false;
            bool trimWhitespace = false;
            var buffer = new StringBuilder();
            var node = source.Read();
            while (true)
            {
                if (node == -1)
                {
                    if (buffer.Length > 0)
                    {
                        if (inExpression)
                        {
                            throw new InvalidOperationException("Reached end of template before expression was closed");
                        }
                        else
                        {
                            yield return Token.Static(buffer.ToString());
                        }
                    }
                    break;
                }
                if (inExpression)
                {
                    if ((char)node == '(')
                    {
                        yield return Token.StartSubExpression();
                    }

                    Token token = null;
                    token = token ?? WordParser.Parse(source);
                    token = token ?? LiteralParser.Parse(source);
                    token = token ?? CommentParser.Parse(source);
                    token = token ?? PartialParser.Parse(source);
                    token = token ?? BlockWordParser.Parse(source);
                    token = token ?? BlockParamsParser.Parse(source);

                    if (token != null)
                    {
                        yield return token;
                            
                        if ((char)source.Peek() == '=')
                        {
                            source.Read();
                            yield return Token.Assignment();
                            continue;
                        }
                    }
                    if ((char)node == '}' && (char)source.Read() == '}')
                    {
                        bool escaped = true;
                        bool raw = false;
                        if ((char)source.Peek() == '}')
                        {
                            node = source.Read();
                            escaped = false;
                        }
                        if ((char)source.Peek() == '}')
                        {
                            node = source.Read();
                            raw = true;
                        }
                        node = source.Read();
                        yield return Token.EndExpression(escaped, trimWhitespace, raw);
                        inExpression = false;
                    }
                    else if ((char)node == ')')
                    {
                        node = source.Read();
                        yield return Token.EndSubExpression();
                    }
                    else if (char.IsWhiteSpace((char)node) || char.IsWhiteSpace((char)source.Peek()))
                    {
                        node = source.Read();
                    }
                    else if ((char)node == '~')
                    {
                        node = source.Read();
                        trimWhitespace = true;
                    }
                    else
                    {
                        if (token == null)
                        {
                            
                            throw new HandlebarsParserException("Reached unparseable token in expression: " + source.ReadLine());
                        }
                        node = source.Read();
                    }
                }
                else
                {
                    if ((char)node == '\\' && (char)source.Peek() == '\\')
                    {
                        source.Read();
                        buffer.Append('\\');
                        node = source.Read();
                    }
                    else if ((char)node == '\\' && (char)source.Peek() == '{')
                    {
                        source.Read();
                        if ((char)source.Peek() == '{')
                        {
                            source.Read();
                            buffer.Append('{', 2);
                        }
                        else
                        {
                            buffer.Append("\\{");
                        }
                        node = source.Read();
                    }
                    else if ((char)node == '{' && (char)source.Peek() == '{')
                    {
                        bool escaped = true;
                        bool raw = false;
                        trimWhitespace = false;
                        node = source.Read();
                        if ((char)source.Peek() == '{')
                        {
                            node = source.Read();
                            escaped = false;
                        }
                        if ((char)source.Peek() == '{')
                        {
                            node = source.Read();
                            raw = true;
                        }
                        if ((char)source.Peek() == '~')
                        {
                            source.Read();
                            node = source.Peek();
                            trimWhitespace = true;
                        }
                        yield return Token.Static(buffer.ToString());
                        yield return Token.StartExpression(escaped, trimWhitespace, raw);
                        trimWhitespace = false;
                        buffer = new StringBuilder();
                        inExpression = true;
                    }
                    else
                    {
                        buffer.Append((char)node);
                        node = source.Read();
                    }
                }
            }
        }
    }
}

