using System;
using System.Collections.Generic;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal static class Tokenizer
    {
        private static readonly Parser WordParser = new WordParser();
        private static readonly Parser LiteralParser = new LiteralParser();
        private static readonly Parser CommentParser = new CommentParser();
        private static readonly Parser PartialParser = new PartialParser();
        private static readonly Parser BlockWordParser = new BlockWordParser();
        private static readonly Parser BlockParamsParser = new BlockParamsParser();
        //TODO: structure parser
        
        public static IEnumerable<Token> Tokenize(ExtendedStringReader source)
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

        private static IEnumerable<Token> Parse(ExtendedStringReader source)
        {
            bool inExpression = false;
            bool trimWhitespace = false;
            using var container = StringBuilderPool.Shared.Use();
            var buffer = container.Value;

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
                            yield return Token.Static(buffer.ToString(), source.GetContext());
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

                    var token = WordParser.Parse(source);
                    token ??= LiteralParser.Parse(source);
                    token ??= CommentParser.Parse(source);
                    token ??= PartialParser.Parse(source);
                    token ??= BlockWordParser.Parse(source);
                    token ??= BlockParamsParser.Parse(source);

                    if (token != null)
                    {
                        yield return token;

                        if ((char)source.Peek() == '=')
                        {
                            source.Read();
                            yield return Token.Assignment(source.GetContext());
                            continue;
                        }
                    }
                    if ((char)node == '}' && (char)source.Read() == '}')
                    {
                        bool escaped = true;
                        bool raw = false;
                        if ((char)source.Peek() == '}')
                        {
                            source.Read();
                            escaped = false;
                        }
                        if ((char)source.Peek() == '}')
                        {
                            source.Read();
                            raw = true;
                        }
                        node = source.Read();
                        yield return Token.EndExpression(escaped, trimWhitespace, raw, source.GetContext());
                        inExpression = false;
                    }
                    else if ((char)node == ')')
                    {
                        node = source.Read();
                        yield return Token.EndSubExpression(source.GetContext());
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
                            
                            throw new HandlebarsParserException("Reached unparseable token in expression: " + source.ReadLine(), source.GetContext());
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
                        yield return Token.Static(buffer.ToString(), source.GetContext());
                        yield return Token.StartExpression(escaped, trimWhitespace, raw, source.GetContext());
                        trimWhitespace = false;
                        buffer.Clear();
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

