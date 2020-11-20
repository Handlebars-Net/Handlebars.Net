using System;

namespace HandlebarsDotNet.IO.Formatters.DefaultFormatters
{
    public class DefaultDoubleFormatter : IFormatter
    {
        public void Format<T>(T value, in EncodedTextWriter writer)
        {
            if(!(value is double v)) throw new ArgumentException(" supposed to be double", nameof(value));
            writer.UnderlyingWriter.Write(v);
        }
    }
}