using System;
using System.Diagnostics.CodeAnalysis;

namespace HandlebarsDotNet.IO.Formatters.DefaultFormatters
{
    public class DefaultUIntFormatter : IFormatter
    {
        public void Format<T>([NotNull] T value, in EncodedTextWriter writer)
        {
            if(!(value is uint v)) throw new ArgumentException(" supposed to be uint", nameof(value));
            writer.UnderlyingWriter.Write(v);
        }
    }
}