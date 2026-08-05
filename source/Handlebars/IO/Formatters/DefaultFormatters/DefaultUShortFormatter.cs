using System;
using System.Diagnostics.CodeAnalysis;

namespace HandlebarsDotNet.IO.Formatters.DefaultFormatters
{
    public class DefaultUShortFormatter : IFormatter
    {
        public void Format<T>([NotNull] T value, in EncodedTextWriter writer)
        {
            if(!(value is ushort v)) throw new ArgumentException(" supposed to be ushort", nameof(value));
            writer.UnderlyingWriter.Write(v);
        }
    }
}