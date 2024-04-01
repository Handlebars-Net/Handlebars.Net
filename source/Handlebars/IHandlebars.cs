using System.IO;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.BlockHelpers;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet
{
    public delegate string HandlebarsTemplate<in TContext, in TData>(TContext context, TData data = null) 
        where TData: class
        where TContext: class;
    
    public delegate void HandlebarsTemplate<in TWriter, in TContext, in TData>(TWriter writer, TContext context, TData data = null)
        where TWriter: TextWriter
        where TData: class
        where TContext: class;
    
    /// <summary>
    /// 
    /// </summary>
    public interface IHandlebars : IHelpersRegistry
    {
        /// <summary>
        /// Creates shared Handlebars environment that is used to compile templates that share the same configuration
        /// <para>Runtime only changes can be applied after object creation!</para>
        /// </summary>
        IHandlebars CreateSharedEnvironment();
        
        bool IsSharedEnvironment { get; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        HandlebarsTemplate<TextWriter, object, object> Compile(TextReader template);

        HandlebarsTemplate<object, object> Compile(string template);
        
        HandlebarsTemplate<object, object> CompileView(string templatePath);
        
        HandlebarsTemplate<TextWriter, object, object> CompileView(string templatePath, ViewReaderFactory readerFactoryFactory);
        
        HandlebarsConfiguration Configuration { get; }
        
        void RegisterTemplate(string templateName, HandlebarsTemplate<TextWriter, object, object> template);
        
        void RegisterTemplate(string templateName, string template);

        void RegisterDecorator(string helperName, HandlebarsBlockDecorator helperFunction);
        
        void RegisterDecorator(string helperName, HandlebarsDecorator helperFunction);
        
        void RegisterDecorator(string helperName, HandlebarsBlockDecoratorVoid helperFunction);
        
        void RegisterDecorator(string helperName, HandlebarsDecoratorVoid helperFunction);

        /// <summary>
        /// Defines current environment configuration scope.
        /// <para>It's not required to call this method. However it enables certain optimizations.</para>
        /// <para>Example:<code>
        /// var env = Handlebars.Create();
        /// using(env.Configure())
        /// {
        ///    env.RegisterHelper(...)
        /// }
        /// </code></para>
        /// </summary>
        /// <returns></returns>
        DisposableContainer Configure();
    }
    
    internal interface ICompiledHandlebars
    {
        ICompiledHandlebarsConfiguration CompiledConfiguration { get; }
    }
}

