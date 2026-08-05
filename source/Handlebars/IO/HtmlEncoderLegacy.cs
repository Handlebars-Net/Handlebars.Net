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
            var start = 0;
            var anyEscaped = false;
            for (var index = 0; index < length; index++)
            {
                var value = text[index];
                if (!RequiresEscaping(value)) continue;

                anyEscaped = true;
                if (index != start) WriteRun(text, start, index - start, target);
                WriteEscaped(value, target);
                start = index + 1;
            }

            if (!anyEscaped)
            {
                // Fast path: nothing to escape, write the whole string at once
                target.Write(text);
                return;
            }

            if (start < length) WriteRun(text, start, length - start, target);
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

        private static void WriteEscaped(char value, TextWriter target)
        {
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
                    target.Write("&#");
                    target.Write((int)value);
                    target.Write(";");
                    break;
            }
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
