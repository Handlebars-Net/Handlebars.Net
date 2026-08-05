using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using HandlebarsDotNet.StringUtils;

namespace HandlebarsDotNet
{
    /// <summary>
    /// <inheritdoc />
    /// Produces <c>HTML</c> safe output.
    /// </summary>
    public class HtmlEncoder : ITextEncoder
    {
        /*
         * Escape set based on: https://github.com/handlebars-lang/handlebars.js/blob/master/lib/handlebars/utils.js
         * As of 2021-12-20 / commit https://github.com/handlebars-lang/handlebars.js/commit/3fb331ef40ee1a8308dd83b8e5adbcd798d0adc9
         */
        private static readonly char[] EscapeChars = { '&', '<', '>', '"', '\'', '`', '=' };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Encode(StringBuilder text, TextWriter target)
        {
            if(text == null || text.Length == 0) return;

            EncodeImpl(new StringBuilderEnumerator(text), target);
        }

        public void Encode(string text, TextWriter target)
        {
            if(string.IsNullOrEmpty(text)) return;

            var index = text.IndexOfAny(EscapeChars);
            if (index == -1)
            {
                // Fast path: nothing to escape, write the whole string at once
                target.Write(text);
                return;
            }

            var start = 0;
            do
            {
                var runLength = index - start;
                if (runLength != 0) WriteRun(text, start, runLength, target);
                target.Write(GetEscapeSequence(text[index]));

                start = index + 1;
                index = start < text.Length ? text.IndexOfAny(EscapeChars, start) : -1;
            } while (index != -1);

            if (start < text.Length) WriteRun(text, start, text.Length - start, target);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Encode<T>(T text, TextWriter target) where T : IEnumerator<char>
        {
            if (text is null) return;

            EncodeImpl(text, target);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteRun(string text, int start, int length, TextWriter target)
        {
#if NETSTANDARD2_0
            var end = start + length;
            for (var i = start; i < end; i++)
            {
                target.Write(text[i]);
            }
#else
            target.Write(text.AsSpan(start, length));
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetEscapeSequence(char value)
        {
            switch (value)
            {
                case '&': return "&amp;";
                case '<': return "&lt;";
                case '>': return "&gt;";
                case '"': return "&quot;";
                case '\'': return "&#x27;";
                case '`': return "&#x60;";
                default: return "&#x3D;"; // '='
            }
        }

        private static void EncodeImpl<T>(T text, TextWriter target) where T : IEnumerator<char>
        {
            /*
             * Based on: https://github.com/handlebars-lang/handlebars.js/blob/master/lib/handlebars/utils.js
             * As of 2021-12-20 / commit https://github.com/handlebars-lang/handlebars.js/commit/3fb331ef40ee1a8308dd83b8e5adbcd798d0adc9
             */
            while (text.MoveNext())
            {
                var value = text.Current;
                switch (value)
                {
                    case '&':
                        target.Write("&amp;");
                        break;
                    case '<':
                        target.Write("&lt;");
                        break;
                    case '>':
                        target.Write("&gt;");
                        break;
                    case '"':
                        target.Write("&quot;");
                        break;
                    case '\'':
                        target.Write("&#x27;");
                        break;
                    case '`':
                        target.Write("&#x60;");
                        break;
                    case '=':
                        target.Write("&#x3D;");
                        break;
                    default:
                        target.Write(value);
                        break;
                }
            }
        }
    }
}
