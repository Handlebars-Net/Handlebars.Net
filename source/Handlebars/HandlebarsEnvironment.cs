using System;
using System.IO;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.Compiler;

namespace HandlebarsDotNet
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="templatePath"></param>
    public delegate TextReader ViewReaderFactory(ICompiledHandlebarsConfiguration configuration, string templatePath);
    
    internal class HandlebarsEnvironment : IHandlebars
    {
        private static readonly ViewReaderFactory ViewReaderFactory = (configuration, path) =>
        {
            var fs = configuration.FileSystem;
            if (fs == null)
                throw new InvalidOperationException("Cannot compile view when configuration.FileSystem is not set");
            var template = fs.GetFileContent(path);
            if (template == null)
                throw new InvalidOperationException("Cannot find template at '" + path + "'");
                
            return new StringReader(template);
        };
        
        public HandlebarsEnvironment(HandlebarsConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public Action<TextWriter, object> CompileView(string templatePath, ViewReaderFactory readerFactoryFactory)
        {
            readerFactoryFactory = readerFactoryFactory ?? ViewReaderFactory;
            return CompileViewInternal(templatePath, readerFactoryFactory);
        }

        public Func<object,string> CompileView(string templatePath)
        {
            var view = CompileViewInternal(templatePath, ViewReaderFactory);
            return (vm) =>
            {
                using (var writer = new PolledStringWriter(Configuration.FormatProvider))
                {
                    view(writer, vm);
                    return writer.ToString();
                }
            };
        }

        private Action<TextWriter, object> CompileViewInternal(string templatePath, ViewReaderFactory readerFactoryFactory)
        {
            var configuration = new InternalHandlebarsConfiguration(Configuration);
            var createdFeatures = configuration.Features;
            for (var index = 0; index < createdFeatures.Count; index++)
            {
                createdFeatures[index].OnCompiling(configuration);
            }

            var compiler = new HandlebarsCompiler(configuration);
            var compiledView = compiler.CompileView(readerFactoryFactory, templatePath, configuration);
    
            for (var index = 0; index < createdFeatures.Count; index++)
            {
                createdFeatures[index].CompilationCompleted();
            }

            return compiledView;
        }

        public HandlebarsConfiguration Configuration { get; }

        public Action<TextWriter, object> Compile(TextReader template)
        {
            using (var reader = new ExtendedStringReader(template))
            {
                var compiler = new HandlebarsCompiler(Configuration);
                return compiler.Compile(reader);
            }
        }

        public Func<object, string> Compile(string template)
        {
            using (var reader = new StringReader(template))
            {
                var compiledTemplate = Compile(reader);
                return context =>
                {
                    using (var writer = new PolledStringWriter(Configuration.FormatProvider))
                    {
                        compiledTemplate(writer, context);
                        return writer.ToString();
                    }
                };
            }
        }

        public void RegisterTemplate(string templateName, Action<TextWriter, object> template)
        {
            Configuration.RegisteredTemplates[templateName] = template;
        }

        public void RegisterTemplate(string templateName, string template)
        {
            using (var reader = new StringReader(template))
            {
                RegisterTemplate(templateName, Compile(reader));
            }
        }

        public void RegisterHelper(string helperName, HandlebarsHelper helperFunction)
        {
            Configuration.Helpers[helperName] = helperFunction;
        }
            
        public void RegisterHelper(string helperName, HandlebarsReturnHelper helperFunction)
        {
            Configuration.ReturnHelpers[helperName] = helperFunction;
        }

        public void RegisterHelper(string helperName, HandlebarsBlockHelper helperFunction)
        {
            Configuration.BlockHelpers[helperName] = helperFunction;
        }
    }
}
