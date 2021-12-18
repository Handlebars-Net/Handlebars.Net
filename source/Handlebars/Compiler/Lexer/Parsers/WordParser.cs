using System.Collections.Generic;
using System.Text;
using HandlebarsDotNet.Extensions;
using HandlebarsDotNet.Pools;
using HandlebarsDotNet.StringUtils;

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
            if (!IsWord(reader)) return null;
            
            var buffer = AccumulateWord(reader);
            return Token.Word(buffer, context);
        }

        private static bool IsWord(ExtendedStringReader reader)
        {
            var peek = reader.Peek();
            return ValidWordStartCharacters.Contains((char) peek);
        }

        private static string AccumulateWord(ExtendedStringReader reader)
        {
            using var container = StringBuilderPool.Shared.Use();
            var buffer = container.Value;
                
            var inString = false;
            var isEscaped = false;

            while (true)
            {
                if (isEscaped)
                {
                    var c = (char) reader.Read();
                    if (c == ']') isEscaped = false;
                    
                    buffer.Append(c);
                    continue;
                }
                
                if (!inString)
                {
                    var peek = (char) reader.Peek();

                    if (peek == '}' || peek == '~' || peek == ')' || peek == '=' || char.IsWhiteSpace(peek))
                    {
                        break;
                    }
                }

                var node = reader.Read();

                if (node == -1)
                {
                    throw new HandlebarsParserException("Reached end of template before the expression was closed.", reader.GetContext());
                }

                if (node == '[' && !inString)
                {
                    isEscaped = true;
                    buffer.Append((char)node);
                    continue;
                }
                
                if (node == '\'' || node == '"')
                {
                    inString = !inString;
                }

                buffer.Append((char)node);
            }
                
            return buffer.Trim().ToString();
        }
        
        private static bool CanBreakAtSpace(StringBuilder buffer)
        {
            var left = 0;
            var right = 0;
            
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

            return left == 0 || left == right;
        }
    }
}

