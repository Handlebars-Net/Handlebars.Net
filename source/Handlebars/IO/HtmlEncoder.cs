using System.Globalization;

namespace HandlebarsDotNet
{
    /// <summary>
    /// <inheritdoc />
    /// Produces <c>HTML</c> safe output.
    /// </summary>
    public class HtmlEncoder : ITextEncoder
    {
        /// <inheritdoc />
        public string Encode(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;


            // Detect if we need to allocate a stringbuilder and new string
            for (var i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '"':
                    case '&':
                    case '<':
                    case '>':
                        return ReallyEncode(text, i);
                    default:
                        if (text[i] > 159)
                        {
                            return ReallyEncode(text, i);
                        }
                        else

                            break;
                }
            }

            return text;
        }

        private static string ReallyEncode(string text, int i)
        {
            using (var container = StringBuilderPool.Shared.Use())
            {
                var sb = container.Value;
                sb.Append(text, 0, i);
                for (; i < text.Length; i++)
                {
                    switch (text[i])
                    {
                        case '"':
                            sb.Append("&quot;");
                            break;
                        case '&':
                            sb.Append("&amp;");
                            break;
                        case '<':
                            sb.Append("&lt;");
                            break;
                        case '>':
                            sb.Append("&gt;");
                            break;

                        default:
                            if (text[i] > 159)
                            {
                                sb.Append("&#");
                                sb.Append(((int)text[i]).ToString(CultureInfo.InvariantCulture));
                                sb.Append(";");
                            }
                            else
                                sb.Append(text[i]);

                            break;
                    }
                }

                return sb.ToString();
            }
        }
    }
}