using System;

namespace HandlebarsDotNet.IO.Formatters.DefaultFormatters
{
    public class DefaultShortFormatter : IFormatter
    {
        public void Format<T>(T value, in EncodedTextWriter writer)
        {
            if(!(value is short v)) throw new ArgumentException(" supposed to be short", nameof(value));
            writer.UnderlyingWriter.Write(v);
        }
    }
}