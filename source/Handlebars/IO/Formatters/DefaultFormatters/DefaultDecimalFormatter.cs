using System;

namespace HandlebarsDotNet.IO.Formatters.DefaultFormatters
{
    public class DefaultDecimalFormatter : IFormatter
    {
        public void Format<T>(T value, in EncodedTextWriter writer)
        {
            if(!(value is decimal v)) throw new ArgumentException(" supposed to be decimal", nameof(value));
            writer.UnderlyingWriter.Write(v);
        }
    }
}