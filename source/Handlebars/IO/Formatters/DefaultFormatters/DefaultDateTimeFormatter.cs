using System;
using System.Diagnostics.CodeAnalysis;

namespace HandlebarsDotNet.IO.Formatters.DefaultFormatters
{
    public class DefaultDateTimeFormatter : IFormatter
    {
        public void Format<T>([NotNull] T value, in EncodedTextWriter writer)
        {
            if(!(value is DateTime dateTime)) throw new ArgumentException(" supposed to be DateTime", nameof(value));
            writer.Write(dateTime.ToString("O"), false);
        }
    }
}