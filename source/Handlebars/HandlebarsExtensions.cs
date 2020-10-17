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
            if (value is string str)
            {
                writer.Write(new SafeString(str));
                return;
            }
            
            writer.Write(new SafeString(value.ToString()));
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
        
        public static object This(this HelperOptions options, object context, Func<HelperOptions, Action<TextWriter, object>> selector)
        {
            using var writer = ReusableStringWriter.Get(options.Configuration.FormatProvider);
            selector(options)(writer, context);
            return writer.ToString();
        }
    }

    
    public interface ISafeString
    {
        string Value { get; }
    }
    
    public class SafeString : ISafeString
    {
        public string Value { get; }
        
        public SafeString(string value) => Value = value;

        public override string ToString() => Value;
    }
}

