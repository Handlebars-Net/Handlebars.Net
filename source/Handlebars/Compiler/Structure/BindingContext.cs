using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.ValueProviders;
using Microsoft.Extensions.ObjectPool;

namespace HandlebarsDotNet.Compiler
{
    internal sealed class BindingContext : IDisposable
    {
        private static readonly BindingContextPool Pool = new BindingContextPool();
        
        private readonly HashedCollection<IValueProvider> _valueProviders = new HashedCollection<IValueProvider>();

        public static BindingContext Create(InternalHandlebarsConfiguration configuration, object value,
            EncodedTextWriter writer, BindingContext parent, string templatePath,
            IDictionary<string, Action<TextWriter, object>> inlinePartialTemplates)
        {
            return Pool.CreateContext(configuration, value, writer, parent, templatePath, null, inlinePartialTemplates);
        } 
        
        public static BindingContext Create(InternalHandlebarsConfiguration configuration, object value,
            EncodedTextWriter writer, BindingContext parent, string templatePath,
            Action<TextWriter, object> partialBlockTemplate,
            IDictionary<string, Action<TextWriter, object>> inlinePartialTemplates)
        {
            return Pool.CreateContext(configuration, value, writer, parent, templatePath, partialBlockTemplate, inlinePartialTemplates);
        }

        private BindingContext()
        {
            RegisterValueProvider(new BindingContextValueProvider(this));
        }

        private void Initialize()
        {
            Root = ParentContext?.Root ?? this;
            TemplatePath = (ParentContext != null ? ParentContext.TemplatePath : TemplatePath) ?? TemplatePath;
            
            //Inline partials cannot use the Handlebars.RegisteredTemplate method
            //because it pollutes the static dictionary and creates collisions
            //where the same partial name might exist in multiple templates.
            //To avoid collisions, pass around a dictionary of compiled partials
            //in the context
            if (ParentContext != null)
            {
                InlinePartialTemplates = ParentContext.InlinePartialTemplates;

                if (Value is HashParameterDictionary dictionary) {
                    // Populate value with parent context
                    foreach (var item in GetContextDictionary(ParentContext.Value)) {
                        if (!dictionary.ContainsKey(item.Key))
                            dictionary[item.Key] = item.Value;
                    }
                }
            }
            else
            {
                InlinePartialTemplates = new Dictionary<string, Action<TextWriter, object>>(StringComparer.OrdinalIgnoreCase);
            }
        }

        public string TemplatePath { get; private set; }

        public InternalHandlebarsConfiguration Configuration { get; private set; }
        
        public EncodedTextWriter TextWriter { get; private set; }

        public IDictionary<string, Action<TextWriter, object>> InlinePartialTemplates { get; private set; }

        public Action<TextWriter, object> PartialBlockTemplate { get; private set; }
        
        public bool SuppressEncoding
        {
            get => TextWriter.SuppressEncoding;
            set => TextWriter.SuppressEncoding = value;
        }

        public object Value { get; private set; }

        public BindingContext ParentContext { get; private set; }

        public object Root { get; private set; }

        public void RegisterValueProvider(IValueProvider valueProvider)
        {
            if(valueProvider == null) throw new ArgumentNullException(nameof(valueProvider));
            
            _valueProviders.Add(valueProvider);
        }
        
        public void UnregisterValueProvider(IValueProvider valueProvider)
        {
            _valueProviders.Remove(valueProvider);
        }

        public bool TryGetContextVariable(ref ChainSegment segment, out object value)
        {
            // accessing value providers in reverse order as it gives more probability of hit
            for (var index = _valueProviders.Count - 1; index >= 0; index--)
            {
                if (_valueProviders[index].TryGetValue(ref segment, out value)) return true;
            }

            value = null;
            return false;
        }
        
        public bool TryGetVariable(ref ChainSegment segment, out object value, bool searchContext = false)
        {
            // accessing value providers in reverse order as it gives more probability of hit
            for (var index = _valueProviders.Count - 1; index >= 0; index--)
            {
                var valueProvider = _valueProviders[index];
                if(!valueProvider.SupportedValueTypes.HasFlag(ValueTypes.All) && !searchContext) continue;
                
                if (valueProvider.TryGetValue(ref segment, out value)) return true;
            }

            value = null;
            return ParentContext?.TryGetVariable(ref segment, out value, searchContext) ?? false;
        }

        private static IDictionary<string, object> GetContextDictionary(object target)
        {
            var contextDictionary = new Dictionary<string, object>();

            switch (target)
            {
                case null:
                    return contextDictionary;
                
                case IDictionary<string, object> dictionary:
                {
                    foreach (var item in dictionary)
                    {
                        contextDictionary[item.Key] = item.Value;
                    }

                    break;
                }
                default:
                {
                    var type = target.GetType();

                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var field in fields) 
                    {
                        contextDictionary[field.Name] = field.GetValue(target);
                    }

                    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var property in properties) 
                    {
                        if (property.GetIndexParameters().Length == 0)
                        {
                            contextDictionary[property.Name] = property.GetValue(target);
                        }
                    }

                    break;
                }
            }

            return contextDictionary;
        }

        public BindingContext CreateChildContext(object value, Action<TextWriter, object> partialBlockTemplate = null)
        {
            return Create(Configuration, value ?? Value, TextWriter, this, TemplatePath, partialBlockTemplate ?? PartialBlockTemplate, null);
        }
        
        public void Dispose()
        {
            Pool.Return(this);
        }
        
        private class BindingContextPool : DefaultObjectPool<BindingContext>
        {
            public BindingContextPool() : base(new BindingContextPolicy())
            {
            }
            
            public BindingContext CreateContext(InternalHandlebarsConfiguration configuration, object value, EncodedTextWriter writer, BindingContext parent, string templatePath, Action<TextWriter, object> partialBlockTemplate, IDictionary<string, Action<TextWriter, object>> inlinePartialTemplates)
            {
                var context = Get();
                context.Configuration = configuration;
                context.Value = value;
                context.TextWriter = writer;
                context.ParentContext = parent;
                context.TemplatePath = templatePath;
                context.InlinePartialTemplates = inlinePartialTemplates;
                context.PartialBlockTemplate = partialBlockTemplate;

                context.Initialize();

                return context;
            }
        
            private class BindingContextPolicy : IPooledObjectPolicy<BindingContext>
            {
                public BindingContext Create()
                {
                    return new BindingContext();
                }

                public bool Return(BindingContext item)
                {
                    item.Root = null;
                    item.Value = null;
                    item.ParentContext = null;
                    item.TemplatePath = null;
                    item.TextWriter = null;
                    item.InlinePartialTemplates = null;
                    item.PartialBlockTemplate = null;

                    var valueProviders = item._valueProviders;
                    for (var index = valueProviders.Count - 1; index >= 1; index--)
                    {
                        valueProviders.Remove(valueProviders[index]);
                    }

                    return true;
                }
            }
        }
    }
}
