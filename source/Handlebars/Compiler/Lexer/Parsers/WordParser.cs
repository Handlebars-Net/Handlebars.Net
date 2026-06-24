using System.Collections.Generic;
using System.Text;
using HandlebarsDotNet.Extensions;
using HandlebarsDotNet.Pools;
using HandlebarsDotNet.StringUtils;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class WordParser : Parser
    {
        private const string ValidWordStartCharactersString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_$.@[]*";
        private static readonly HashSet<char> ValidWordStartCharacters = new HashSet<char>();

        // Invisible Unicode characters that should be stripped from identifiers
        // Includes BOM (U+FEFF), zero-width space (U+200B), zero-width non-joiner (U+200C),
        // zero-width joiner (U+200D), word joiner (U+2060), and other format characters.
        private static bool IsInvisibleCharacter(char c)
        {
            return c == '﻿' // BOM / Zero Width No-Break Space
                || c == '​' // Zero Width Space
                || c == '‌' // Zero Width Non-Joiner
                || c == '‍' // Zero Width Joiner
                || c == '⁠' // Word Joiner
                || c == '᠎' // Mongolian Vowel Separator
                || char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.Format;
        }

        static WordParser()
        {
            for (var index = 0; index < ValidWordStartCharactersString.Length; index++)
            {
                ValidWordStartCharacters.Add(ValidWordStartCharactersString[index]);
            }
        }

        public override Token? Parse(ExtendedStringReader reader)
        {
            var context = reader.GetContext();
            if (!IsWord(reader)) return null;

            var buffer = AccumulateWord(reader);
            return Token.Word(buffer, context);
        }

        private static bool IsWord(ExtendedStringReader reader)
        {
            var peek = reader.Peek();
            if (peek == -1) return false;
            var c = (char) peek;
            return ValidWordStartCharacters.Contains(c) || char.IsLetter(c);
        }

        private static string AccumulateWord(ExtendedStringReader reader)
        {
            using var container = StringBuilderPool.Shared.Use();
            var buffer = container.Value;

            var inString = false;
            var isEscaped = false;

            while (true)
            {
                if (!inString && !isEscaped)
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

                if (isEscaped)
                {
                    var c = (char) node;
                    if (c == ']') isEscaped = false;

                    buffer.Append(c);
                    continue;
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

                // Skip invisible Unicode characters (BOM, zero-width chars, etc.)
                // that can appear in identifiers due to editor/encoding artifacts
                if (!inString && IsInvisibleCharacter((char) node))
                {
                    continue;
                }

                buffer.Append((char)node);
            }

            return buffer.Trim().ToString();
        }
    }
}

