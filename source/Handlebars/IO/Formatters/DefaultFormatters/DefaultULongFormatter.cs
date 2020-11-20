using System;

namespace HandlebarsDotNet.IO.Formatters.DefaultFormatters
{
    public class DefaultULongFormatter : IFormatter
    {
        public void Format<T>(T value, in EncodedTextWriter writer)
        {
            if(!(value is ulong v)) throw new ArgumentException(" supposed to be ulong", nameof(value));
            writer.UnderlyingWriter.Write(v);
        }
    }
}