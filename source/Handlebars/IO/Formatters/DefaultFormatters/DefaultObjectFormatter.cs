using System.Diagnostics.CodeAnalysis;

namespace HandlebarsDotNet.IO.Formatters.DefaultFormatters
{
    public class DefaultObjectFormatter : IFormatter
    {
        public void Format<T>([NotNull] T value, in EncodedTextWriter writer)
        {
            writer.Write(value!.ToString());
        }
    }
}