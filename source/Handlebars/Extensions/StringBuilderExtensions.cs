using System.Text;
using HandlebarsDotNet.StringUtils;

namespace HandlebarsDotNet.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder Trim(this StringBuilder builder, char @char = ' ') 
            => builder.TrimStart(@char).TrimEnd(@char);

        public static StringBuilder TrimStart(this StringBuilder builder, char @char = ' ')
        {
            var count = 0;
            for (var index = 0; index < builder.Length; index++)
            {
                if (builder[index] == @char) { count++; continue; }
                break;
            }
            
            return builder.Remove(0, count);
        }
        
        public static StringBuilder TrimEnd(this StringBuilder builder, char @char = ' ')
        {
            var count = 0;
            for (var index = builder.Length - 1; index >= 0; index--)
            {
                if (builder[index] == @char) { count++; continue; }
                break;
            }

            return builder.Remove(builder.Length - count, count);
        }
        
        public static StringBuilder Append(this StringBuilder builder, in Substring substring) 
            => builder.Append(substring.String, substring.Start, substring.Length);
    }
}