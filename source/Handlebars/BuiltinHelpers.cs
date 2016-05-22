using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace HandlebarsDotNet
{
	internal static class BuiltinHelpers
    {
        [Description("with")]
        public static void With(TextWriter output, HelperOptions options, dynamic context, params object[] arguments)
        {
            if (arguments.Length != 1)
            {
                throw new HandlebarsException("{{with}} helper must have exactly one argument"); 
            }
            options.Template(output, arguments[0]);
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
#if netstandard
            foreach (var method in builtInHelpersType.GetTypeInfo().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public))

#else
            foreach (var method in builtInHelpersType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public))
#endif
            {
                Delegate possibleDelegate;
	            try
	            {
#if netstandard
                        possibleDelegate = method.CreateDelegate(typeof(T), method);
#else
                        possibleDelegate = Delegate.CreateDelegate(typeof(T), method); 
#endif
	            }
	            catch
	            {
		            possibleDelegate = null;
	            }
	            if (possibleDelegate != null)
                {
#if netstandard
                    yield return new KeyValuePair<string, T>(
                        method.GetCustomAttribute<DescriptionAttribute>().Description,
                        (T)(object)possibleDelegate);
#else
                    yield return new KeyValuePair<string, T>(
                        ((DescriptionAttribute)Attribute.GetCustomAttribute(method, typeof(DescriptionAttribute))).Description,
                        (T)(object)possibleDelegate);
#endif
                }
            }
        }
    }
}

