using System;

namespace HandlebarsDotNet.IO.Formatters.DefaultFormatters
{
    public class DefaultFloatFormatter : IFormatter
    {
        public void Format<T>(T value, in EncodedTextWriter writer)
        {
            if(!(value is float v)) throw new ArgumentException(" supposed to be float", nameof(value));
            writer.UnderlyingWriter.Write(v);
        }
    }
}