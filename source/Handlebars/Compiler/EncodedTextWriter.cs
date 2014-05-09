using System;
using System.IO;
using System.Net;
using System.Text;

namespace Handlebars
{
    internal class EncodedTextWriter : TextWriter
    {
        public override void Write(string value)
        {
            base.Write(WebUtility.HtmlEncode(value));
        }

        public override void Write(object value)
        {
            if(value is ISafeString)
            {
                base.Write(value.ToString());
            }
            else
            {
                base.Write(value);
            }
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}

