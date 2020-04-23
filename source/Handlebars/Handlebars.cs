using System;
using System.IO;

namespace HandlebarsDotNet
{
    /// <summary>
    /// InlineHelper: {{#helper arg1 arg2}}
    /// </summary>
    /// <param name="output"></param>
    /// <param name="context"></param>
    /// <param name="arguments"></param>
    public delegate void HandlebarsHelper(TextWriter output, dynamic context, params object[] arguments);
    
    /// <summary>
    /// InlineHelper: {{#helper arg1 arg2}}, supports <see cref="object"/> value return
    /// </summary>
    /// <param name="context"></param>
    /// <param name="arguments"></param>
    public delegate object HandlebarsReturnHelper(dynamic context, params object[] arguments);
    
    /// <summary>
    /// BlockHelper: {{#helper}}..{{/helper}}
    /// </summary>
    /// <param name="output"></param>
    /// <param name="options"></param>
    /// <param name="context"></param>
    /// <param name="arguments"></param>
    public delegate void HandlebarsBlockHelper(TextWriter output, HelperOptions options, dynamic context, params object[] arguments);

    /// <summary>
    /// 
    /// </summary>
    public sealed class Handlebars
    {
        // Lazy-load Handlebars environment to ensure thread safety.  See Jon Skeet's excellent article on this for more info. http://csharpindepth.com/Articles/General/Singleton.aspx
        private static readonly Lazy<IHandlebars> Lazy = new Lazy<IHandlebars>(() => new HandlebarsEnvironment(new HandlebarsConfiguration()));

        private static IHandlebars Instance => Lazy.Value;

        /// <summary>
        /// Creates standalone instance of <see cref="Handlebars"/> environment
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IHandlebars Create(HandlebarsConfiguration configuration = null)
        {
            configuration = configuration ?? new HandlebarsConfiguration();
            return new HandlebarsEnvironment(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static Action<TextWriter, object> Compile(TextReader template)
        {
            return Instance.Compile(template);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static Func<object, string> Compile(string template)
        {
            return Instance.Compile(template);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="templatePath"></param>
        /// <returns></returns>
        public static Func<object, string> CompileView(string templatePath)
        {
            return Instance.CompileView(templatePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="template"></param>
        public static void RegisterTemplate(string templateName, Action<TextWriter, object> template)
        {
            Instance.RegisterTemplate(templateName, template);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="template"></param>
        public static void RegisterTemplate(string templateName, string template)
        {
            Instance.RegisterTemplate(templateName, template);
        }

        /// <summary>
        /// Registers new <see cref="HandlebarsHelper"/>
        /// </summary>
        /// <param name="helperName"></param>
        /// <param name="helperFunction"></param>
        [Obsolete("Consider switching to HandlebarsReturnHelper")]
        public static void RegisterHelper(string helperName, HandlebarsHelper helperFunction)
        {
            Instance.RegisterHelper(helperName, helperFunction);
        }
        
        /// <summary>
        /// Registers new <see cref="HandlebarsReturnHelper"/>
        /// </summary>
        /// <param name="helperName"></param>
        /// <param name="helperFunction"></param>
        public static void RegisterHelper(string helperName, HandlebarsReturnHelper helperFunction)
        {
            Instance.RegisterHelper(helperName, helperFunction);
        }

        /// <summary>
        /// Registers new <see cref="HandlebarsBlockHelper"/>
        /// </summary>
        /// <param name="helperName"></param>
        /// <param name="helperFunction"></param>
        public static void RegisterHelper(string helperName, HandlebarsBlockHelper helperFunction)
        {
            Instance.RegisterHelper(helperName, helperFunction);
        }

        /// <summary>
        /// Expose the configuration in order to have access in all Helpers and Templates.
        /// </summary>
        public static HandlebarsConfiguration Configuration => Instance.Configuration;
    }
}