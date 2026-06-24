using System;
using System.Diagnostics.CodeAnalysis;

namespace HandlebarsDotNet.IO.Formatters.DefaultFormatters
{
    public class DefaultBoolFormatter : IFormatter
    {
        public void Format<T>([NotNull] T value, in EncodedTextWriter writer)
        {
            if(!(value is bool v)) throw new ArgumentException(" supposed to be bool", nameof(value));
            writer.UnderlyingWriter.Write(v);
        }
    }
}