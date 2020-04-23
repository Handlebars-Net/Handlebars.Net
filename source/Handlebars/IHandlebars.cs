using System;
using System.IO;

namespace HandlebarsDotNet
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHandlebars
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        Action<TextWriter, object> Compile(TextReader template);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        Func<object, string> Compile(string template);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templatePath"></param>
        /// <returns></returns>
        Func<object, string> CompileView(string templatePath);

        /// <summary>
        /// 
        /// </summary>
        HandlebarsConfiguration Configuration { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="template"></param>
        void RegisterTemplate(string templateName, Action<TextWriter, object> template);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="template"></param>
        void RegisterTemplate(string templateName, string template);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="helperName"></param>
        /// <param name="helperFunction"></param>
        [Obsolete("Consider switching to HandlebarsReturnHelper")]
        void RegisterHelper(string helperName, HandlebarsHelper helperFunction);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="helperName"></param>
        /// <param name="helperFunction"></param>
        void RegisterHelper(string helperName, HandlebarsReturnHelper helperFunction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="helperName"></param>
        /// <param name="helperFunction"></param>
        void RegisterHelper(string helperName, HandlebarsBlockHelper helperFunction);
    }
}

