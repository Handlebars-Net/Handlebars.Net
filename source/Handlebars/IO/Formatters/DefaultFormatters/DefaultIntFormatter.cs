using System;

namespace HandlebarsDotNet.IO.Formatters.DefaultFormatters
{
    public class DefaultIntFormatter : IFormatter
    {
        public void Format<T>(T value, in EncodedTextWriter writer)
        {
            if(!(value is int v)) throw new ArgumentException(" supposed to be int", nameof(value));
            writer.UnderlyingWriter.Write(v);
        }
    }
}