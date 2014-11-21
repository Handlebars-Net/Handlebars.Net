using System;
using System.IO;
using System.Net;
using System.Text;

namespace Handlebars
{
    internal class EncodedTextWriter : TextWriter
    {
        private readonly TextWriter _underlyingWriter;

        public EncodedTextWriter(TextWriter writer)
        {
            _underlyingWriter = writer;
        }

        public override void Write(string value)
        {
            _underlyingWriter.Write(WebUtility.HtmlEncode(value));
        }

        public override void Write(object value)
        {
            if (value == null)
            {
                return;
            }
            if (value is ISafeString)
            {
                _underlyingWriter.Write(value.ToString());
            }
            else
            {
                this.Write(value.ToString());
            }
        }

        public TextWriter UnderlyingWriter
        {
            get { return _underlyingWriter; }
        }

        public override Encoding Encoding
        {
            get { return _underlyingWriter.Encoding; }
        }
    }
}

