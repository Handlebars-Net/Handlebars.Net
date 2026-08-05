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
#if NET8_0_OR_GREATER
        private static readonly System.Buffers.SearchValues<char> EscapeChars = System.Buffers.SearchValues.Create("&<>\"'`=");
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Encode(StringBuilder text, TextWriter target)
        {
            if(text == null || text.Length == 0) return;

            EncodeImpl(new StringBuilderEnumerator(text), target);
        }

        public void Encode(string text, TextWriter target)
        {
            if(string.IsNullOrEmpty(text)) return;

            var index = IndexOfEscapeChar(text, 0);
            if (index == -1)
            {
                // Fast path: nothing to escape, write the whole string at once
                target.Write(text);
                return;
            }

            // Bulk-write the clean prefix, then fall back to per-character encoding.
            // Escape-dense content makes per-segment bulk writes counter-productive:
            // the fixed cost of a span write outweighs a few Write(char) calls.
            if (index != 0) WriteRun(text, 0, index, target);
            EncodeImpl(new StringEnumerator(text, index), target);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int IndexOfEscapeChar(string text, int start)
        {
#if NET8_0_OR_GREATER
            // SearchValues pre-computes the lookup structure once; string.IndexOfAny(char[])
            // with more than 5 needles would rebuild a probabilistic map on every call.
            var index = text.AsSpan(start).IndexOfAny(EscapeChars);
            return index < 0 ? -1 : index + start;
#else
            for (var i = start; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '&':
                    case '<':
                    case '>':
                    case '"':
                    case '\'':
                    case '`':
                    case '=':
                        return i;
                }
            }

            return -1;
#endif
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
#if !NETSTANDARD2_0
            // StringWriter and StreamWriter override Write(ReadOnlySpan<char>) with efficient
            // implementations; for other writers TextWriter's base implementation rents and
            // copies through ArrayPool, so they keep the original per-character writes.
            if (target is StringWriter || target is StreamWriter)
            {
                target.Write(text.AsSpan(start, length));
                return;
            }
#endif
            var end = start + length;
            for (var i = start; i < end; i++)
            {
                target.Write(text[i]);
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
