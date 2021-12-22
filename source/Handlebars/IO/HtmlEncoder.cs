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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Encode(StringBuilder text, TextWriter target)
        {
            if(text == null || text.Length == 0) return;
            
            EncodeImpl(new StringBuilderEnumerator(text), target);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Encode(string text, TextWriter target)
        {
            if(string.IsNullOrEmpty(text)) return;
            
            EncodeImpl(new StringEnumerator(text), target);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Encode<T>(T text, TextWriter target) where T : IEnumerator<char>
        {
            if(text == null) return;
            
            EncodeImpl(text, target);
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
