using System;
using System.Diagnostics.CodeAnalysis;

namespace HandlebarsDotNet.IO
{
    public sealed class UndefinedFormatter : IFormatter, IFormatterProvider
    {
        public UndefinedFormatter(string? formatString = null) => FormatString = formatString;

        public string? FormatString { get; set; }
        
        public bool TryCreateFormatter(Type type, [MaybeNullWhen(false)] out IFormatter formatter)
        {
            if (type != typeof(UndefinedBindingResult))
            {
                formatter = null;
                return false;
            }

            formatter = this;
            return true;
        }
        
        public void Format<T>(T value, in EncodedTextWriter writer)
        {
            if (FormatString is not {Length: > 0}) return;
            
            writer.Write(FormatString, (value as UndefinedBindingResult)!.Value);
        }
    }
}