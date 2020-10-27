using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using HandlebarsDotNet.Compiler.Lexer;
using HandlebarsDotNet.StringUtils;

namespace HandlebarsDotNet
{
    /// <summary>
    /// <inheritdoc />
    /// Produces <c>HTML</c> safe output.
    /// </summary>
    public class HtmlEncoder : ITextEncoder
    {
        public HtmlEncoder(IFormatProvider provider) => FormatProvider = provider;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldEncode(char c)
        {
            return c == '"'
                   || c == '&'
                   || c == '>'
                   || c == '<'
                   || c > 159;
        }

        public IFormatProvider FormatProvider { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Encode(StringBuilder text, TextWriter target)
        {
            if(text == null || text.Length == 0) return;
            
            EncodeImpl(new StringBuilderWrapper(text), target);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Encode(string text, TextWriter target)
        {
            if(string.IsNullOrEmpty(text)) return;
            
            EncodeImpl(new StringWrapper(text), target);
        }
        
        private static void EncodeImpl<T>(T text, TextWriter target) where T: IStringWrapper
        {
            for (var i = 0; i < text.Count; i++)
            {
                var value = text[i];
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