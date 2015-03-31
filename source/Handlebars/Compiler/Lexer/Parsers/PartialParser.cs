using System;
using System.IO;
using System.Text;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class PartialParser : Parser
    {
        public override Token Parse(TextReader reader)
        {
            PartialToken token = null;
            if ((char)reader.Peek() == '>')
            {
                var buffer = AccumulatePartial(reader);
                token = Token.Partial(buffer);
            }
            return token;
        }

        private string AccumulatePartial(TextReader reader)
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append(">");
            do
            {
                reader.Read();
            }
            while(char.IsWhiteSpace((char)reader.Peek()));
            while(true)
            {
                var peek = (char)reader.Peek();
                if (peek == '}' || char.IsWhiteSpace(peek))
                {
                    break;
                }
                var node = reader.Read();
                if (node == -1)
                {
                    throw new InvalidOperationException("Reached end of template before the expression was closed.");
                }
                else
                {
                    buffer.Append((char)node);
                }
            }
            return buffer.ToString();
        }
    }
}

