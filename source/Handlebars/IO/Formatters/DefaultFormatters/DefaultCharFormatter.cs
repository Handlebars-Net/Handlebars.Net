using System;
using System.Diagnostics.CodeAnalysis;

namespace HandlebarsDotNet.IO.Formatters.DefaultFormatters
{
    public class DefaultCharFormatter : IFormatter
    {
        public void Format<T>([NotNull] T value, in EncodedTextWriter writer)
        {
            if(!(value is char v)) throw new ArgumentException(" supposed to be char", nameof(value));
            writer.UnderlyingWriter.Write(v);
        }
    }
}