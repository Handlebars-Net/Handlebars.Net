using System;
using System.Diagnostics.CodeAnalysis;

namespace HandlebarsDotNet.IO.Formatters.DefaultFormatters
{
    public class DefaultULongFormatter : IFormatter
    {
        public void Format<T>([NotNull] T value, in EncodedTextWriter writer)
        {
            if(!(value is ulong v)) throw new ArgumentException(" supposed to be ulong", nameof(value));
            writer.UnderlyingWriter.Write(v);
        }
    }
}