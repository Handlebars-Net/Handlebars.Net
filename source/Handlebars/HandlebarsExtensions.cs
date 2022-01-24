using System;
using System.IO;

namespace HandlebarsDotNet
{
    public static class HandlebarsExtensions
    {
        /// <summary>
        /// Writes an encoded string using <see cref="ITextEncoder"/>
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteSafeString(this in EncodedTextWriter writer, string value)
        {
            writer.Write(value, false);
        }

        /// <summary>
        /// Writes an encoded string using <see cref="ITextEncoder"/>
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteSafeString(this in EncodedTextWriter writer, object value)
        {
            if (value is string str)
            {
                writer.WriteSafeString(str);
                return;
            }

            var current = writer.SuppressEncoding;
            try
            {
                writer.SuppressEncoding = true;
                writer.Write(value);
            }
            finally
            {
                writer.SuppressEncoding = current;
            }
        }
        
        /// <summary>
        /// Allows configuration manipulations
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static HandlebarsConfiguration Configure(this HandlebarsConfiguration configuration, Action<HandlebarsConfiguration> config)
        {
            config(configuration);

            return configuration;
        }
    }
}

