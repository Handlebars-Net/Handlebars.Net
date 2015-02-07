using System;
using System.IO;

namespace Handlebars
{
    public delegate void HandlebarsHelper(TextWriter output, dynamic context, params object[] arguments);
    public delegate void HandlebarsBlockHelper(TextWriter output, HelperOptions options, dynamic context, params object[] arguments);

    public sealed partial class Handlebars
    {
        private static readonly IHandlebars _singleton = new HandlebarsEnvironment(new HandlebarsConfiguration());

        public static IHandlebars Create(HandlebarsConfiguration configuration = null)
        {
            configuration = configuration ?? new HandlebarsConfiguration();
            return new HandlebarsEnvironment(configuration);
        }

        public static Action<TextWriter, object> Compile(TextReader template)
        {
            return _singleton.Compile(template);
        }

        public static Func<object, string> Compile(string template)
        {
            return _singleton.Compile(template);
        }

        public static void RegisterTemplate(string templateName, Action<TextWriter, object> template)
        {
            _singleton.RegisterTemplate(templateName, template);
        }

        public static void RegisterHelper(string helperName, HandlebarsHelper helperFunction)
        {
            _singleton.RegisterHelper(helperName, helperFunction);
        }

        public static void RegisterHelper(string helperName, HandlebarsBlockHelper helperFunction)
        {
            _singleton.RegisterHelper(helperName, helperFunction);
        }

        /// <summary>
        /// Expose the configuration on order to have access in all Helpers and Templates.
        /// </summary>
        public static HandlebarsConfiguration Configuration
        {
            get { return _singleton.Configuration; }
        }
    }
}