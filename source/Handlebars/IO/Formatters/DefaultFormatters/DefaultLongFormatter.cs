using System;

namespace HandlebarsDotNet.IO.Formatters.DefaultFormatters
{
    public class DefaultLongFormatter : IFormatter
    {
        public void Format<T>(T value, in EncodedTextWriter writer)
        {
            if(!(value is long v)) throw new ArgumentException(" supposed to be long", nameof(value));
            writer.UnderlyingWriter.Write(v);
        }
    }
}