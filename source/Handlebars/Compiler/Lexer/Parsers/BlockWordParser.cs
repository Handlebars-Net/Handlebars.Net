using System.Collections.Generic;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class BlockWordParser : Parser
    {
        private static readonly HashSet<char> ValidBlockWordStartCharacters = new HashSet<char>
        {
            '#', '^', '/'
        };

        public override Token Parse(ExtendedStringReader reader)
        {
            if (!IsBlockWord(reader)) return null;

            var context = reader.GetContext();
            var buffer = AccumulateBlockWord(reader);
            var token = Token.Word(buffer, context);
            return token;
        }

        private static bool IsBlockWord(ExtendedStringReader reader)
        {
            var peek = (char)reader.Peek();
            return ValidBlockWordStartCharacters.Contains(peek);
        }

        private static string AccumulateBlockWord(ExtendedStringReader reader)
        {
            using(var container = StringBuilderPool.Shared.Use())
            {
                var buffer = container.Value;
                buffer.Append((char)reader.Read());
                while(char.IsWhiteSpace((char)reader.Peek()))
                {
                    reader.Read();
                }
                
                while(true)
                {
                    var peek = (char)reader.Peek();
                    if (peek == '}' || peek == '~' || char.IsWhiteSpace(peek))
                    {
                        break;
                    }
                    var node = reader.Read();
                    if (node == -1)
                    {
                        throw new HandlebarsParserException("Reached end of template before the expression was closed.", reader.GetContext());
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
}

