using System;
using System.IO;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.BlockHelpers;

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
        
        Func<object, string> Compile(string template);
        
        Func<object, string> CompileView(string templatePath);
        
        Action<TextWriter, object> CompileView(string templatePath, ViewReaderFactory readerFactoryFactory);
        
        HandlebarsConfiguration Configuration { get; }
        
        void RegisterTemplate(string templateName, Action<TextWriter, object> template);
        
        void RegisterTemplate(string templateName, string template);
        
        void RegisterHelper(string helperName, HandlebarsHelper helperFunction);
        
        void RegisterHelper(string helperName, HandlebarsReturnHelper helperFunction);
        
        void RegisterHelper(string helperName, HandlebarsBlockHelper helperFunction);
        
        void RegisterHelper(string helperName, HandlebarsReturnBlockHelper helperFunction);
        
        void RegisterHelper(BlockHelperDescriptor helperObject);
        
        void RegisterHelper(HelperDescriptor helperObject);
        
        void RegisterHelper(ReturnBlockHelperDescriptor helperObject);
        
        void RegisterHelper(ReturnHelperDescriptor helperObject);
    }
    
    internal interface ICompiledHandlebars
    {
        ICompiledHandlebarsConfiguration CompiledConfiguration { get; }
    }
}

