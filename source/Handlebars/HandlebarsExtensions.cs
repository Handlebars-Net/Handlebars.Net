using System;
using System.IO;

namespace Handlebars
{
    public static class HandlebarsExtensions
    {
        public static void WriteSafeString(this TextWriter writer, string value)
        {
            writer.Write(new SafeString(value));
        }
            
        private class SafeString : ISafeString
        {
            private readonly string _value;

            public SafeString(string value)
            {
                _value = value;
            }

            public override string ToString()
            {
                return _value;
            }
        }
    }
}

