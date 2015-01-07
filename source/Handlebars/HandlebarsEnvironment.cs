using System;
using System.IO;
using System.Text;
using Handlebars.Compiler;

namespace Handlebars
{
    public class HandlebarsEnvironment : IHandlebars
    {
        private readonly HandlebarsConfiguration _configuration;
        private readonly HandlebarsCompiler _compiler;

        public HandlebarsEnvironment(HandlebarsConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            _configuration = configuration;
            _compiler = new HandlebarsCompiler(_configuration);
            RegisterBuiltinHelpers();
        }

        public HandlebarsConfiguration Configuration
        {
            get
            {
                return this._configuration;
            }
        }

        public Action<TextWriter, object> Compile(TextReader template)
        {
            return _compiler.Compile(template);
        }

        public Func<object, string> Compile(string template)
        {
            using (var reader = new StringReader(template))
            {
                var compiledTemplate = Compile(reader);
                return context =>
                {
                    var builder = new StringBuilder();
                    using (var writer = new StringWriter(builder))
                    {
                        compiledTemplate(writer, context);
                    }
                    return builder.ToString();
                };
            }
        }

        public void RegisterTemplate(string templateName, Action<TextWriter, object> template)
        {
            if (_configuration.RegisteredTemplates.ContainsKey(templateName) == false)
            {
                lock (typeof(Handlebars))
                {
                    if (_configuration.RegisteredTemplates.ContainsKey(templateName) == false)
                    {
                        _configuration.RegisteredTemplates.Add(templateName, template);
                    }
                }
            }
        }

        public void RegisterHelper(string helperName, HandlebarsHelper helperFunction)
        {
            if (_configuration.Helpers.ContainsKey(helperName) == false)
            {
                lock (typeof(Handlebars))
                {
                    if (_configuration.Helpers.ContainsKey(helperName) == false)
                    {
                        _configuration.Helpers.Add(helperName, helperFunction);
                    }
                }
            }
        }

        public void RegisterHelper(string helperName, HandlebarsBlockHelper helperFunction)
        {
            if (_configuration.BlockHelpers.ContainsKey(helperName) == false)
            {
                lock (typeof(Handlebars))
                {
                    if (_configuration.BlockHelpers.ContainsKey(helperName) == false)
                    {
                        _configuration.BlockHelpers.Add(helperName, helperFunction);
                    }
                }
            }
        }

        private void RegisterBuiltinHelpers()
        {
            foreach (var helperDefinition in BuiltinHelpers.Helpers)
            {
                RegisterHelper(helperDefinition.Key, helperDefinition.Value);
            }
            foreach (var helperDefinition in BuiltinHelpers.BlockHelpers)
            {
                RegisterHelper(helperDefinition.Key, helperDefinition.Value);
            }
        }
    }
}