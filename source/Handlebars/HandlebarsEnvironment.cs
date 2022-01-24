using System;
using System.IO;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Decorators;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.BlockHelpers;
using HandlebarsDotNet.IO;
using HandlebarsDotNet.ObjectDescriptors;
using HandlebarsDotNet.Runtime;

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

        private readonly AmbientContext _ambientContext = AmbientContext.Create();

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
            using var container = AmbientContext.Use(_ambientContext);
            
            var configuration = CompiledConfiguration ?? new HandlebarsConfigurationAdapter(Configuration);
            
            var formatterProvider = new FormatterProvider(configuration.FormatterProviders);
            var objectDescriptorFactory = new ObjectDescriptorFactory(configuration.ObjectDescriptorProviders);
            
            var localContext = AmbientContext.Create(
                _ambientContext, 
                formatterProvider: formatterProvider,
                descriptorFactory: objectDescriptorFactory
            );
            
            using var localContainer = AmbientContext.Use(localContext);
            
            var createdFeatures = configuration.Features;
            for (var index = 0; index < createdFeatures.Count; index++)
            {
                createdFeatures[index].OnCompiling(configuration);
            }

            var compilationContext = new CompilationContext(configuration);
            var compiledView = HandlebarsCompiler.CompileView(readerFactoryFactory, templatePath, compilationContext);
    
            for (var index = 0; index < createdFeatures.Count; index++)
            {
                createdFeatures[index].CompilationCompleted();
            }

            return (writer, context, data) =>
            {
                using var disposableContainer = AmbientContext.Use(localContext);
                
                if (context is BindingContext bindingContext)
                {
                    bindingContext.Extensions["templatePath"] = templatePath; 
                    var config = bindingContext.Configuration;
                    using var encodedTextWriter = new EncodedTextWriter(writer, config.TextEncoder, formatterProvider, config.NoEscape);
                    compiledView(encodedTextWriter, bindingContext);
                }
                else
                {
                    using var newBindingContext = BindingContext.Create(configuration, context);
                    newBindingContext.Extensions["templatePath"] = templatePath;
                    newBindingContext.SetDataObject(data);

                    using var encodedTextWriter = new EncodedTextWriter(writer, configuration.TextEncoder, formatterProvider, configuration.NoEscape);
                    compiledView(encodedTextWriter, newBindingContext);
                }
            };
        }

        public HandlebarsTemplate<TextWriter, object, object> Compile(TextReader template)
        {
            using var container = AmbientContext.Use(_ambientContext);
            
            var configuration = CompiledConfiguration ?? new HandlebarsConfigurationAdapter(Configuration);
            
            var formatterProvider = new FormatterProvider(configuration.FormatterProviders);
            var objectDescriptorFactory = new ObjectDescriptorFactory(configuration.ObjectDescriptorProviders);
            
            var localContext = AmbientContext.Create(
                _ambientContext, 
                formatterProvider: formatterProvider,
                descriptorFactory: objectDescriptorFactory
            );
            
            using var localContainer = AmbientContext.Use(localContext);
            
            var compilationContext = new CompilationContext(configuration);
            using var reader = new ExtendedStringReader(template);
            var compiledTemplate = HandlebarsCompiler.Compile(reader, compilationContext);
            
            return (writer, context, data) =>
            {
                using var disposableContainer = AmbientContext.Use(localContext);

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
                        using var encodedTextWriter = new EncodedTextWriter(writer, config.TextEncoder, formatterProvider, config.NoEscape);
                        compiledTemplate(encodedTextWriter, bindingContext);
                    }
                    else
                    {
                        using var newBindingContext = BindingContext.Create(configuration, context);
                        newBindingContext.SetDataObject(data);

                        using var encodedTextWriter = new EncodedTextWriter(writer, configuration.TextEncoder, formatterProvider, configuration.NoEscape);
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

        public void RegisterDecorator(string helperName, HandlebarsBlockDecorator helperFunction)
        {
            Configuration.BlockDecorators[$"*{helperName}"] = new DelegateBlockDecoratorDescriptor(helperName, helperFunction);
        }

        public void RegisterDecorator(string helperName, HandlebarsDecorator helperFunction)
        {
            Configuration.Decorators[$"*{helperName}"] = new DelegateDecoratorDescriptor(helperName, helperFunction);
        }
        
        public void RegisterDecorator(string helperName, HandlebarsBlockDecoratorVoid helperFunction)
        {
            Configuration.BlockDecorators[$"*{helperName}"] = new DelegateBlockDecoratorVoidDescriptor(helperName, helperFunction);
        }

        public void RegisterDecorator(string helperName, HandlebarsDecoratorVoid helperFunction)
        {
            Configuration.Decorators[$"*{helperName}"] = new DelegateDecoratorVoidDescriptor(helperName, helperFunction);
        }

        public DisposableContainer Configure()
        {
            return AmbientContext.Use(_ambientContext);
        }

        public IIndexed<string, IHelperDescriptor<HelperOptions>> GetHelpers()
        {
            return Configuration.Helpers;
        }

        public IIndexed<string, IHelperDescriptor<BlockHelperOptions>> GetBlockHelpers()
        {
            return Configuration.BlockHelpers;
        }
    }
}
