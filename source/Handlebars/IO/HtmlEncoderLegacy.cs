using HandlebarsDotNet.StringUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace HandlebarsDotNet
{
    /// <summary>
    /// <inheritdoc />
    /// Produces <c>HTML</c> safe output.
    /// <para>
    /// This will encode non-ascii characters.
    /// this will not encode '=', '`' or ''' (single quote).
    /// </para>
    /// </summary>
    public class HtmlEncoderLegacy : ITextEncoder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Encode(StringBuilder text, TextWriter target)
        {
            if (text == null || text.Length == 0) return;

            EncodeImpl(new StringBuilderEnumerator(text), target);
        }

        public void Encode(string text, TextWriter target)
        {
            if (string.IsNullOrEmpty(text)) return;

            var length = text.Length;
            var index = 0;
            while (index < length && !RequiresEscaping(text[index])) index++;

            if (index == length)
            {
                // Fast path: nothing to escape, write the whole string at once
                target.Write(text);
                return;
            }

            // Bulk-write the clean prefix, then fall back to per-character encoding.
            if (index != 0) WriteRun(text, 0, index, target);
            EncodeImpl(new StringEnumerator(text, index), target);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Encode<T>(T text, TextWriter target) where T : IEnumerator<char>
        {
            if (text is null) return;

            EncodeImpl(text, target);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool RequiresEscaping(char value)
        {
            switch (value)
            {
                case '"':
                case '&':
                case '<':
                case '>':
                    return true;
                default:
                    return value > 159;
            }
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
            while (text.MoveNext())
            {
                var value = text.Current;
                switch (value)
                {
                    case '"':
                        target.Write("&quot;");
                        break;
                    case '&':
                        target.Write("&amp;");
                        break;
                    case '<':
                        target.Write("&lt;");
                        break;
                    case '>':
                        target.Write("&gt;");
                        break;

                    default:
                        if (value > 159)
                        {
                            target.Write("&#");
                            target.Write((int)value);
                            target.Write(";");
                        }
                        else target.Write(value);
                        break;
                }
            }
        }
    }
}
