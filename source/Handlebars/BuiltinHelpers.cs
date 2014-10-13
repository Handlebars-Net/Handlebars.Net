using System;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;

namespace Handlebars
{
    internal static class BuiltinHelpers
    {
        [Description("with")]
        public static void With(TextWriter output, Action<TextWriter, object> template, dynamic context, params object[] arguments)
        {
            if(arguments.Length != 1)
            {
                throw new HandlebarsException("{{with}} helper must have exactly one argument"); 
            }
            template(output, arguments[0]);
        }

        public static IEnumerable<KeyValuePair<string, HandlebarsHelper>> Helpers
        {
            get
            {
                return GetHelpers<HandlebarsHelper>();
            }
        }

        public static IEnumerable<KeyValuePair<string, HandlebarsBlockHelper>> BlockHelpers
        {
            get
            {
                return GetHelpers<HandlebarsBlockHelper>();
            }
        }

        private static IEnumerable<KeyValuePair<string, T>> GetHelpers<T>()
        {
            var builtInHelpersType = typeof(BuiltinHelpers);
            foreach(var method in builtInHelpersType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public))
            {
                var possibleDelegate = Delegate.CreateDelegate(typeof(T), method, false);
                if(possibleDelegate != null)
                {
                    yield return new KeyValuePair<string, T>(
                        ((DescriptionAttribute)Attribute.GetCustomAttribute (method, typeof (DescriptionAttribute))).Description,
                        (T)(object)possibleDelegate);
                }
            }
        }
    }
}

