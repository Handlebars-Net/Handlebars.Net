using System;
using System.IO;
using Handlebars.Compiler;
using System.Text;

namespace Handlebars
{
    public delegate void HandlebarsHelper(TextWriter output, dynamic context, params object[] arguments);
    public delegate void HandlebarsBlockHelper(TextWriter output, Action<TextWriter, object> template, dynamic context, params object[] arguments);

    public sealed class Handlebars
    {
        private static readonly IHandlebars _singleton = new HandlebarsEnvironment(new HandlebarsConfiguration());

        public static IHandlebars Create()
        {
            return new HandlebarsEnvironment(new HandlebarsConfiguration());
        }

        public static Action<TextWriter, object> Compile(TextReader template)
        {
            return _singleton.Compile(template);
        }

        public static Func<object, string> Compile(string template)
        {
            return _singleton.Compile(template);
        }

        public static void RegisterTemplate(string templateName, Action<TextWriter, object> template)
        {
            _singleton.RegisterTemplate(templateName, template);
        }

        public static void ClearRegisteredTemplates()
        {
            _singleton.ClearRegisteredTemplates();
        }

        public static void RegisterHelper(string helperName, HandlebarsHelper helperFunction)
        {
            _singleton.RegisterHelper(helperName, helperFunction);
        }

        public static void RegisterHelper(string helperName, HandlebarsBlockHelper helperFunction)
        {
            _singleton.RegisterHelper(helperName, helperFunction);
        }

        private class HandlebarsEnvironment : IHandlebars
        {
            private readonly HandlebarsConfiguration _configuration;
            private readonly HandlebarsCompiler _compiler;

            public HandlebarsEnvironment(HandlebarsConfiguration configuration)
            {
                _configuration = configuration;
                _compiler = new HandlebarsCompiler(_configuration);
                RegisterBuiltinHelpers();
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

            public void ClearRegisteredTemplates()
            {
                if (_configuration.RegisteredTemplates.Count > 0)
                {
                    lock (typeof(Handlebars))
                    {
                        if (_configuration.RegisteredTemplates.Count > 0)
                        {
                            _configuration.RegisteredTemplates.Clear();
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
}

