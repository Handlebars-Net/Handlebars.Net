using System;
using System.IO;
using System.Linq;
using System.Text;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class WordParser : Parser
    {
        private const string validWordStartCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_$.@";

        public override Token Parse(TextReader reader)
        {
            WordExpressionToken token = null;
            if (IsWord(reader))
            {
                var buffer = AccumulateWord(reader);
                token = Token.Word(buffer);
            }
            return token;
        }

        private bool IsWord(TextReader reader)
        {
            var peek = (char)reader.Peek();
            return validWordStartCharacters.Contains(peek.ToString());
        }

        private string AccumulateWord(TextReader reader)
        {
            StringBuilder buffer = new StringBuilder();
            while (true)
            {
                var peek = (char)reader.Peek();
                if (peek == '}' || peek == '~' || char.IsWhiteSpace(peek))
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

