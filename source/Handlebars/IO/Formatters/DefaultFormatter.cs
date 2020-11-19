using System;

namespace HandlebarsDotNet.IO
{
    public class DefaultFormatter : IFormatter, IFormatterProvider
    {
        public void Format<T>(T value, in EncodedTextWriter writer)
        {
            switch (value)
            {
                case bool v: writer.UnderlyingWriter.Write(v); return;
                case int v: writer.UnderlyingWriter.Write(v); return;
                case char v: writer.UnderlyingWriter.Write(v); return;
                case float v: writer.UnderlyingWriter.Write(v); return;
                case double v: writer.UnderlyingWriter.Write(v); return;
                case long v: writer.UnderlyingWriter.Write(v); return;
                case short v: writer.UnderlyingWriter.Write(v); return;
                case uint v: writer.UnderlyingWriter.Write(v); return;
                case ulong v: writer.UnderlyingWriter.Write(v); return;
                case ushort v: writer.UnderlyingWriter.Write(v); return;
                case decimal v: writer.UnderlyingWriter.Write(v); return;

                default: writer.Write(value.ToString()); return;
            }
        }

        public bool TryCreateFormatter(Type type, out IFormatter formatter)
        {
            formatter = this;
            return true;
        }
    }
}