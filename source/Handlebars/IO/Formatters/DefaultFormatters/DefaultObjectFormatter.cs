namespace HandlebarsDotNet.IO.Formatters.DefaultFormatters
{
    public class DefaultObjectFormatter : IFormatter
    {
        public void Format<T>(T value, in EncodedTextWriter writer)
        {
            writer.Write(value.ToString());
        }
    }
}