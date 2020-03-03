using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using HandlebarsDotNet.Compiler;

namespace HandlebarsDotNet
{
    internal abstract class ObjectPool<T>
    {
        private readonly ConcurrentQueue<T> _objects;

        protected ObjectPool()
        {
            _objects = new ConcurrentQueue<T>();
        }

        public T GetObject()
        {
            return _objects.TryDequeue(out var item) 
                ? item 
                : CreateObject();
        }

        public virtual void PutObject(T item)
        {
            _objects.Enqueue(item);
        }

        protected abstract T CreateObject();
    }

    internal class StringBuilderPool : ObjectPool<StringBuilder>
    {
        private static readonly Lazy<StringBuilderPool> Lazy = new Lazy<StringBuilderPool>(() => new StringBuilderPool());
        
        private readonly int _initialCapacity;

        public static StringBuilderPool Shared => Lazy.Value;
        
        public StringBuilderPool(int initialCapacity = 100)
        {
            _initialCapacity = initialCapacity;
        }
        
        protected override StringBuilder CreateObject()
        {
            return new StringBuilder(_initialCapacity);
        }

        public override void PutObject(StringBuilder item)
        {
            item.Length = 0;
            base.PutObject(item);
        }
    }
    
    public partial class Handlebars
    {
        private class HandlebarsEnvironment : IHandlebars
        {
            private readonly HandlebarsCompiler _compiler;

            public HandlebarsEnvironment(HandlebarsConfiguration configuration)
            {
                Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
                _compiler = new HandlebarsCompiler(Configuration);
                RegisterBuiltinHelpers();
            }

            public Func<object, string> CompileView(string templatePath)
            {
                var compiledView = _compiler.CompileView(templatePath);
                return (vm) =>
                {
                    var sb = StringBuilderPool.Shared.GetObject();
                    try
                    {
                        using (var tw = new StringWriter(sb))
                        {
                            compiledView(tw, vm);
                        }
                        return sb.ToString();
                    }
                    finally
                    {
                        StringBuilderPool.Shared.PutObject(sb);
                    }
                };
            }

            public HandlebarsConfiguration Configuration { get; }

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
                        var builder = StringBuilderPool.Shared.GetObject();
                        try
                        {
                            using (var writer = new StringWriter(builder))
                            {
                                compiledTemplate(writer, context);
                            }
                            return builder.ToString();
                        }
                        finally
                        {
                            StringBuilderPool.Shared.PutObject(builder);
                        }
                    };
                }
            }

            public void RegisterTemplate(string templateName, Action<TextWriter, object> template)
            {
                lock (Configuration)
                {
                    Configuration.RegisteredTemplates.AddOrUpdate(templateName, template);
                }
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
                lock (Configuration)
                {
                    Configuration.Helpers.AddOrUpdate(helperName, helperFunction);
                }
            }
            
            public void RegisterHelper(string helperName, HandlebarsReturnHelper helperFunction)
            {
                lock (Configuration)
                {
                    Configuration.ReturnHelpers.AddOrUpdate(helperName, helperFunction);
                }
            }

            public void RegisterHelper(string helperName, HandlebarsBlockHelper helperFunction)
            {
                lock (Configuration)
                {
                    Configuration.BlockHelpers.AddOrUpdate(helperName, helperFunction);
                }
            }

            private void RegisterBuiltinHelpers()
            {
                foreach (var helperDefinition in BuiltinHelpers.Helpers)
                {
                    RegisterHelper(helperDefinition.Key, helperDefinition.Value);
                }
                foreach (var helperDefinition in BuiltinHelpers.ReturnHelpers)
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
