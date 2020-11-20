using System;

namespace HandlebarsDotNet.IO.Formatters.DefaultFormatters
{
    public class DefaultBoolFormatter : IFormatter
    {
        public void Format<T>(T value, in EncodedTextWriter writer)
        {
            if(!(value is bool v)) throw new ArgumentException(" supposed to be bool", nameof(value));
            writer.UnderlyingWriter.Write(v);
        }
    }
}