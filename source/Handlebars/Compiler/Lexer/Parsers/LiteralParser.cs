using System.Linq;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class LiteralParser : Parser
    {
        public override Token Parse(ExtendedStringReader reader)
        {
            var context = reader.GetContext();
            if (IsDelimitedLiteral(reader))
            {
                var delimiter = (char)reader.Read();
                var buffer = AccumulateLiteral(reader, true, delimiter);
                return Token.Literal(buffer, delimiter.ToString(), context);
            }

            if (IsNonDelimitedLiteral(reader))
            {
                var buffer = AccumulateLiteral(reader, false, ' ', ')');
                return Token.Literal(buffer, context: context);
            }

            return null;
        }

        private static bool IsDelimitedLiteral(ExtendedStringReader reader)
        {
            var peek = (char)reader.Peek();
            return peek == '\'' || peek == '"';
        }

        private static bool IsNonDelimitedLiteral(ExtendedStringReader reader)
        {
            var peek = (char)reader.Peek();
            return char.IsDigit(peek) || peek == '-';
        }

        private static string AccumulateLiteral(ExtendedStringReader reader, bool captureDelimiter, params char[] delimiters)
        {
            using(var container = StringBuilderPool.Shared.Use())
            {
                var buffer = container.Value;
                while (true)
                {
                    var node = reader.Peek();
                    if (node == -1)
                    {
                        throw new HandlebarsParserException("Reached end of template before the expression was closed.", reader.GetContext());
                    }

                    if (delimiters.Contains((char)node))
                    {
                        if (captureDelimiter)
                        {
                            reader.Read();
                        }
                        break;
                    }

                    if (!captureDelimiter && (char)node == '}')
                    {
                        break;
                    }

                    buffer.Append((char)reader.Read());
                }
                
                return buffer.ToString();
            }
        }
    }
}

