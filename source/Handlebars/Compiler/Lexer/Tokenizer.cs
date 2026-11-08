using System;
using System.Collections.Generic;
using System.Text;
using HandlebarsDotNet.Pools;

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

                    var token = ParseExpressionToken(source);

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
                        ReadExtraBraces(source, '}', out var escaped, out var raw);
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
                else if ((char)node == '{' && (char)source.Peek() == '{')
                {
                    node = ReadExpressionOpening(source, out var escaped, out var raw, out trimWhitespace);
                    yield return Token.Static(buffer.ToString(), source.GetContext());
                    yield return Token.StartExpression(escaped, trimWhitespace, raw, source.GetContext());
                    trimWhitespace = false;
                    buffer.Clear();
                    inExpression = true;
                }
                else
                {
                    node = ReadStaticCharacter(source, node, buffer);
                }
            }
        }

        private static Token? ParseExpressionToken(ExtendedStringReader source)
        {
            var token = WordParser.Parse(source);
            token ??= LiteralParser.Parse(source);
            token ??= CommentParser.Parse(source);
            token ??= PartialParser.Parse(source);
            token ??= BlockWordParser.Parse(source);
            token ??= BlockParamsParser.Parse(source);
            return token;
        }

        /// <summary>
        /// Consumes up to two further <paramref name="brace"/> characters following a <c>{{</c> / <c>}}</c> pair:
        /// a third brace marks the expression unescaped, a fourth marks it raw.
        /// </summary>
        /// <returns>The last character consumed, or -1 if none were.</returns>
        private static int ReadExtraBraces(ExtendedStringReader source, char brace, out bool escaped, out bool raw)
        {
            var last = -1;
            escaped = true;
            raw = false;
            if ((char)source.Peek() == brace)
            {
                last = source.Read();
                escaped = false;
            }
            if ((char)source.Peek() == brace)
            {
                last = source.Read();
                raw = true;
            }
            return last;
        }

        /// <summary>
        /// Called with the reader positioned on the second '{' of an expression opening.
        /// Consumes the remaining opening braces and an optional '~' and returns the new current node.
        /// </summary>
        private static int ReadExpressionOpening(ExtendedStringReader source, out bool escaped, out bool raw, out bool trimWhitespace)
        {
            var node = source.Read();
            var last = ReadExtraBraces(source, '{', out escaped, out raw);
            if (last != -1)
            {
                node = last;
            }

            trimWhitespace = false;
            if ((char)source.Peek() == '~')
            {
                source.Read();
                node = source.Peek();
                trimWhitespace = true;
            }

            return node;
        }

        /// <summary>
        /// Appends static (non-expression) text for the current node, handling backslash escapes,
        /// and returns the next node.
        /// </summary>
        private static int ReadStaticCharacter(ExtendedStringReader source, int node, StringBuilder buffer)
        {
            if ((char)node == '\\' && (char)source.Peek() == '\\')
            {
                source.Read(); // consume second '\'
                // \\{{ → single literal backslash followed by evaluated expression;
                // \\ not followed by {{ → preserve both backslashes verbatim
                buffer.Append('\\', (char)source.Peek() == '{' ? 1 : 2);
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
            }
            else
            {
                buffer.Append((char)node);
            }

            return source.Read();
        }
    }
}

