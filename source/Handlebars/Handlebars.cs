﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Decorators;
using HandlebarsDotNet.Helpers;

namespace HandlebarsDotNet
{
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
        /// Creates shared Handlebars environment that is used to compile templates that share the same configuration
        /// <para>Runtime only changes can be applied after object creation!</para>
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IHandlebars CreateSharedEnvironment(HandlebarsConfiguration configuration = null)
        {
            configuration ??= new HandlebarsConfiguration();
            return new HandlebarsEnvironment(new HandlebarsConfigurationAdapter(configuration));
        }

        /// <summary>
        /// Creates standalone instance of <see cref="Handlebars"/> environment
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        internal static IHandlebars Create(ICompiledHandlebarsConfiguration configuration)
        {
            configuration ??= new HandlebarsConfigurationAdapter(new HandlebarsConfiguration());
            return new HandlebarsEnvironment(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static HandlebarsTemplate<TextWriter, object, object> Compile(TextReader template)
        {
            return Instance.Compile(template);
        }
        
        public static HandlebarsTemplate<object, object> Compile(string template)
        {
            return Instance.Compile(template);
        }

        public static string Compile(string template, object handlebars)
        {
            var sb = new StringBuilder();

            var handlebarsTemplate = Handlebars.Compile(template);

            sb.Append(handlebarsTemplate(handlebars));

            return sb.ToString();
        }
        
        public static HandlebarsTemplate<object, object> CompileView(string templatePath)
        {
            return Instance.CompileView(templatePath);
        }
        
        public static HandlebarsTemplate<TextWriter, object, object> CompileView(string templatePath, ViewReaderFactory readerFactoryFactory)
        {
            return Instance.CompileView(templatePath, readerFactoryFactory);
        }
        
        public static void RegisterTemplate(string templateName, HandlebarsTemplate<TextWriter, object, object> template)
        {
            Instance.RegisterTemplate(templateName, template);
        }
        
        public static void RegisterTemplate(string templateName, string template)
        {
            Instance.RegisterTemplate(templateName, template);
        }

        /// <summary>
        /// Registers new <see cref="HandlebarsHelper"/>
        /// </summary>
        /// <param name="helperName"></param>
        /// <param name="helperFunction"></param>
        public static void RegisterHelper(string helperName, HandlebarsHelper helperFunction)
        {
            Instance.RegisterHelper(helperName, helperFunction);
        }
        
        /// <summary>
        /// Registers new <see cref="HandlebarsHelperWithOptions"/>
        /// </summary>
        /// <param name="helperName"></param>
        /// <param name="helperFunction"></param>
        public static void RegisterHelper(string helperName, HandlebarsHelperWithOptions helperFunction)
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
        /// Registers new <see cref="HandlebarsReturnWithOptionsHelper"/>
        /// </summary>
        /// <param name="helperName"></param>
        /// <param name="helperFunction"></param>
        public static void RegisterHelper(string helperName, HandlebarsReturnWithOptionsHelper helperFunction)
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
        /// Registers new <see cref="HandlebarsReturnBlockHelper"/>
        /// </summary>
        /// <param name="helperName"></param>
        /// <param name="helperFunction"></param>
        public static void RegisterHelper(string helperName, HandlebarsReturnBlockHelper helperFunction)
        {
            Instance.RegisterHelper(helperName, helperFunction);
        }
        
        /// <summary>
        /// Registers new <see cref="HelperDescriptorBase"/>
        /// </summary>
        /// <param name="helperObject"></param>
        public static void RegisterHelper(IHelperDescriptor<HelperOptions> helperObject)
        {
            Instance.RegisterHelper(helperObject);
        }
        
        /// <summary>
        /// Registers new <see cref="BlockHelperDescriptorBase"/>
        /// </summary>
        /// <param name="helperObject"></param>
        public static void RegisterHelper(IHelperDescriptor<BlockHelperOptions> helperObject)
        {
            Instance.RegisterHelper(helperObject);
        }
        
        public void RegisterDecorator(string helperName, HandlebarsBlockDecorator helperFunction)
        {
            Instance.RegisterDecorator(helperName, helperFunction);
        }

        public void RegisterDecorator(string helperName, HandlebarsDecorator helperFunction)
        {
            Instance.RegisterDecorator(helperName, helperFunction);
        }
        
        public void RegisterDecorator(string helperName, HandlebarsBlockDecoratorVoid helperFunction)
        {
            Instance.RegisterDecorator(helperName, helperFunction);
        }

        public void RegisterDecorator(string helperName, HandlebarsDecoratorVoid helperFunction)
        {
            Instance.RegisterDecorator(helperName, helperFunction);
        }

        /// <summary>
        /// Expose the configuration in order to have access in all Helpers and Templates.
        /// </summary>
        public static HandlebarsConfiguration Configuration => Instance.Configuration;
    }
}