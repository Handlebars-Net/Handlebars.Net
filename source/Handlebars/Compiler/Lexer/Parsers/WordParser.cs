using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class WordParser : Parser
    {
        private const string ValidWordStartCharactersString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_$.@[]";
        private static readonly HashSet<char> ValidWordStartCharacters = new HashSet<char>();

        static WordParser()
        {
            for (var index = 0; index < ValidWordStartCharactersString.Length; index++)
            {
                ValidWordStartCharacters.Add(ValidWordStartCharactersString[index]);
            }
        }

        public override Token Parse(ExtendedStringReader reader)
        {
            var context = reader.GetContext();
            if (IsWord(reader))
            {
                var buffer = AccumulateWord(reader);

                return Token.Word(buffer, context);
            }
            return null;
        }

        private static bool IsWord(ExtendedStringReader reader)
        {
            var peek = reader.Peek();
            return ValidWordStartCharacters.Contains((char) peek);
        }

        private static string AccumulateWord(ExtendedStringReader reader)
        {
            using(var container = StringBuilderPool.Shared.Use())
            {
                var buffer = container.Value;
                
                var inString = false;

                while (true)
                {
                    if (!inString)
                    {
                        var peek = (char) reader.Peek();

                        if (peek == '}' || peek == '~' || peek == ')' || peek == '=' || char.IsWhiteSpace(peek) && CanBreakAtSpace(buffer))
                        {
                            break;
                        }
                    }

                    var node = reader.Read();

                    if (node == -1)
                    {
                        throw new HandlebarsParserException("Reached end of template before the expression was closed.", reader.GetContext());
                    }

                    if (node == '\'' || node == '"')
                    {
                        inString = !inString;
                    }

                    buffer.Append((char)node);
                }
                
                return buffer.ToString().Trim();
            }
        }

        private static bool CanBreakAtSpace(StringBuilder buffer)
        {
            CalculateBraces(buffer, out var left, out var right);

            return left == 0 || left == right;
        }

        private static void CalculateBraces(StringBuilder buffer, out int left, out int right)
        {
            left = 0;
            right = 0;
            
            var enumerator = new StringBuilderEnumerator(buffer);
            while (enumerator.MoveNext())
            {
                switch (enumerator.Current)
                {
                    case ']':
                        right++;
                        break;
                    
                    case '[':
                        left++;
                        break;
                }
            }
        }
    }
}

