using System;

namespace HandlebarsDotNet.IO.Formatters.DefaultFormatters
{
    public class DefaultCharFormatter : IFormatter
    {
        public void Format<T>(T value, in EncodedTextWriter writer)
        {
            if(!(value is char v)) throw new ArgumentException(" supposed to be char", nameof(value));
            writer.UnderlyingWriter.Write(v);
        }
    }
}