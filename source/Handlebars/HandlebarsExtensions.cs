using System.Diagnostics;
using System.IO;

namespace HandlebarsDotNet
{
    /// <summary>
    /// 
    /// </summary>
    public static class HandlebarsExtensions
    {
        /// <summary>
        /// Writes an encoded string using <see cref="ITextEncoder"/>
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteSafeString(this TextWriter writer, string value)
        {
            writer.Write(new SafeString(value));
        }

        /// <summary>
        /// Writes an encoded string using <see cref="ITextEncoder"/>
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteSafeString(this TextWriter writer, object value)
        {
            writer.WriteSafeString(value.ToString());
        }

        [DebuggerDisplay("{_value}")]
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

    /// <summary>
    /// 
    /// </summary>
    public interface ISafeString
    {
    }
}

