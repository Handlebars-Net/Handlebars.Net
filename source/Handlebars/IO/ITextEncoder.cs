using System;
using System.IO;
using System.Text;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Encoder used for output encoding.
    /// </summary>
    public interface ITextEncoder
    {
        IFormatProvider FormatProvider { get; }
        
        void Encode(StringBuilder text, TextWriter target);

        void Encode(string text, TextWriter target);

        bool ShouldEncode(char c);
    }
}