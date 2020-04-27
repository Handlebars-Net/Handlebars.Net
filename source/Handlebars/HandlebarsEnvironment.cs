using System;
using System.IO;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.Compiler;

namespace HandlebarsDotNet
{
    internal class HandlebarsEnvironment : IHandlebars
    {
        public HandlebarsEnvironment(HandlebarsConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public Func<object, string> CompileView(string templatePath)
        {
            var configuration = new InternalHandlebarsConfiguration(Configuration);
            var createdFeatures = configuration.Features;
            for (var index = 0; index < createdFeatures.Count; index++)
            {
                createdFeatures[index].OnCompiling(configuration);
            }
            
            var compiler = new HandlebarsCompiler(configuration);
            var compiledView = compiler.CompileView(templatePath, configuration);
            Func<object, string> action = (vm) =>
            {
                using (var writer = new PolledStringWriter(configuration.FormatProvider))
                {
                    compiledView(writer, vm);
                    return writer.ToString();
                }
            };
            
            for (var index = 0; index < createdFeatures.Count; index++)
            {
                createdFeatures[index].CompilationCompleted();
            }

            return action;
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
