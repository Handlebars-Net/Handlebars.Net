using System;
using System.Diagnostics.CodeAnalysis;

namespace HandlebarsDotNet.IO.Formatters.DefaultFormatters
{
    public class DefaultLongFormatter : IFormatter
    {
        public void Format<T>([NotNull] T value, in EncodedTextWriter writer)
        {
            if(!(value is long v)) throw new ArgumentException(" supposed to be long", nameof(value));
            writer.UnderlyingWriter.Write(v);
        }
    }
}