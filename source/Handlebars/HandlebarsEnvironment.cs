using System;
using System.IO;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.BlockHelpers;

namespace HandlebarsDotNet
{
    public delegate TextReader ViewReaderFactory(ICompiledHandlebarsConfiguration configuration, string templatePath);
    
    internal class HandlebarsEnvironment : IHandlebars, ICompiledHandlebars
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
        
        internal HandlebarsEnvironment(ICompiledHandlebarsConfiguration configuration)
        {
            CompiledConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        
        public HandlebarsConfiguration Configuration { get; }
        internal ICompiledHandlebarsConfiguration CompiledConfiguration { get; }
        ICompiledHandlebarsConfiguration ICompiledHandlebars.CompiledConfiguration => CompiledConfiguration;

        public HandlebarsTemplate<TextWriter, object, object> CompileView(string templatePath, ViewReaderFactory readerFactoryFactory)
        {
            readerFactoryFactory ??= ViewReaderFactory;
            return CompileViewInternal(templatePath, readerFactoryFactory);
        }

        public HandlebarsTemplate<object, object> CompileView(string templatePath)
        {
            var view = CompileViewInternal(templatePath, ViewReaderFactory);
            return (vm, data) =>
            {
                var formatProvider = Configuration?.FormatProvider ?? CompiledConfiguration.FormatProvider;
                using var writer = ReusableStringWriter.Get(formatProvider);
                view(writer, vm, data);
                return writer.ToString();
            };
        }

        private HandlebarsTemplate<TextWriter, object, object> CompileViewInternal(string templatePath, ViewReaderFactory readerFactoryFactory)
        {
            var configuration = CompiledConfiguration ?? new HandlebarsConfigurationAdapter(Configuration);
            var createdFeatures = configuration.Features;
            for (var index = 0; index < createdFeatures.Count; index++)
            {
                createdFeatures[index].OnCompiling(configuration);
            }
            
            var compiledView = HandlebarsCompiler.CompileView(readerFactoryFactory, templatePath, configuration);
    
            for (var index = 0; index < createdFeatures.Count; index++)
            {
                createdFeatures[index].CompilationCompleted();
            }

            return (writer, context, data) =>
            {
                if (context is BindingContext bindingContext)
                {
                    var config = bindingContext.Configuration;
                    using var encodedTextWriter = new EncodedTextWriter(writer, config.TextEncoder, config.UnresolvedBindingFormat, config.NoEscape);
                    compiledView(encodedTextWriter, bindingContext);
                }
                else
                {
                    using var newBindingContext = BindingContext.Create(configuration, context, templatePath);
                    newBindingContext.SetDataObject(data);
                
                    using var encodedTextWriter = new EncodedTextWriter(writer, configuration.TextEncoder, configuration.UnresolvedBindingFormat, configuration.NoEscape);
                    compiledView(encodedTextWriter, newBindingContext);
                }
            };
        }

        public HandlebarsTemplate<TextWriter, object, object> Compile(TextReader template)
        {
            var configuration = CompiledConfiguration ?? new HandlebarsConfigurationAdapter(Configuration);
            using var reader = new ExtendedStringReader(template);
            var compiledTemplate = HandlebarsCompiler.Compile(reader, configuration);
            return (writer, context, data) =>
            {
                if (writer is EncodedTextWriterWrapper encodedTextWriterWrapper)
                {
                    var encodedTextWriter = encodedTextWriterWrapper.UnderlyingWriter;
                    if (context is BindingContext bindingContext)
                    {
                        compiledTemplate(encodedTextWriter, bindingContext);
                        return;
                    }
                
                    using var newBindingContext = BindingContext.Create(configuration, context);
                    newBindingContext.SetDataObject(data);

                    compiledTemplate(encodedTextWriter, newBindingContext);
                }
                else
                {
                    if (context is BindingContext bindingContext)
                    {
                        var config = bindingContext.Configuration;
                        using var encodedTextWriter = new EncodedTextWriter(writer, config.TextEncoder, config.UnresolvedBindingFormat, config.NoEscape);
                        compiledTemplate(encodedTextWriter, bindingContext);
                    }
                    else
                    {
                        using var newBindingContext = BindingContext.Create(configuration, context);
                        newBindingContext.SetDataObject(data);

                        using var encodedTextWriter = new EncodedTextWriter(writer, configuration.TextEncoder, configuration.UnresolvedBindingFormat, configuration.NoEscape);
                        compiledTemplate(encodedTextWriter, newBindingContext);    
                    }  
                }
            };
        }

        public HandlebarsTemplate<object, object> Compile(string template)
        {
            using var reader = new StringReader(template);
            var compiledTemplate = Compile(reader);
            return (context, data) =>
            {
                var formatProvider = Configuration?.FormatProvider ?? CompiledConfiguration?.FormatProvider;
                using var writer = ReusableStringWriter.Get(formatProvider);
                compiledTemplate(writer, context, data);
                return writer.ToString();
            };
        }

        public void RegisterTemplate(string templateName, HandlebarsTemplate<TextWriter, object, object> template)
        {
            var registrations = Configuration ?? (IHandlebarsTemplateRegistrations) CompiledConfiguration;
            registrations.RegisteredTemplates[templateName] = template;
        }

        public void RegisterTemplate(string templateName, string template)
        {
            using var reader = new StringReader(template);
            RegisterTemplate(templateName, Compile(reader));
        }

        public void RegisterHelper(string helperName, HandlebarsHelper helperFunction)
        {
            Configuration.Helpers[helperName] = new DelegateHelperDescriptor(helperName, helperFunction);
        }
            
        public void RegisterHelper(string helperName, HandlebarsReturnHelper helperFunction)
        {
            Configuration.Helpers[helperName] = new DelegateReturnHelperDescriptor(helperName, helperFunction);
        }

        public void RegisterHelper(string helperName, HandlebarsBlockHelper helperFunction)
        {
            Configuration.BlockHelpers[helperName] = new DelegateBlockHelperDescriptor(helperName, helperFunction);
        }
        
        public void RegisterHelper(string helperName, HandlebarsReturnBlockHelper helperFunction)
        {
            Configuration.BlockHelpers[helperName] = new DelegateReturnBlockHelperDescriptor(helperName, helperFunction);
        }

        public void RegisterHelper(BlockHelperDescriptor helperObject)
        {
            Configuration.BlockHelpers[helperObject.Name] = helperObject;
        }
        
        public void RegisterHelper(ReturnBlockHelperDescriptor helperObject)
        {
            Configuration.BlockHelpers[helperObject.Name] = helperObject;
        }

        public void RegisterHelper(HelperDescriptor helperObject)
        {
            Configuration.Helpers[helperObject.Name] = helperObject;
        }
        
        public void RegisterHelper(ReturnHelperDescriptor helperObject)
        {
            Configuration.Helpers[helperObject.Name] = helperObject;
        }
    }
}
