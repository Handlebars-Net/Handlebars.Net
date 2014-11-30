using System;
using System.IO;
using System.Text;

namespace Handlebars.Compiler.Lexer
{
    internal class LiteralParser : Parser
    {
        public override Token Parse(TextReader reader)
        {
            LiteralExpressionToken token = null;
            if (IsDelimitedLiteral(reader) == true)
            {
                char delimiter = (char)reader.Read();
                var buffer = AccumulateLiteral(reader, delimiter);
                token = Token.Literal(buffer, delimiter.ToString());
            }
            else if (IsNonDelimitedLiteral(reader) == true)
            {
                char delimiter = ' ';
                var buffer = AccumulateLiteral(reader, delimiter);
                token = Token.Literal(buffer, delimiter.ToString());
            }
            return token;
        }

        private static bool IsDelimitedLiteral(TextReader reader)
        {
            var peek = (char)reader.Peek();
            return peek == '\'' || peek == '"';
        }

        private static bool IsNonDelimitedLiteral(TextReader reader)
        {
            var peek = (char)reader.Peek();
            return char.IsDigit(peek) || peek == '-';
        }

        private static string AccumulateLiteral(TextReader reader, char delimiter)
        {
            StringBuilder buffer = new StringBuilder();
            while (true)
            {
                var node = reader.Read();
                if (node == -1)
                {
                    throw new InvalidOperationException("Reached end of template before the expression was closed.");
                }
                else
                {
                    if ((char)node == delimiter)
                    {
                        break;
                    }
                    else
                    {
                        buffer.Append((char)node);
                    }
                }
            }
            return buffer.ToString();
        }
    }
}

