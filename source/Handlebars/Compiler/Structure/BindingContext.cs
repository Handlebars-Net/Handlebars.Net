using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HandlebarsDotNet.Compiler
{
    internal class BindingContext
    {
        private readonly List<IValueProvider> _valueProviders = new List<IValueProvider>();
        
        public BindingContext(object value, EncodedTextWriter writer, BindingContext parent, string templatePath, IDictionary<string, Action<TextWriter, object>> inlinePartialTemplates) :
            this(value, writer, parent, templatePath, null, null, inlinePartialTemplates) { }

        public BindingContext(object value, EncodedTextWriter writer, BindingContext parent, string templatePath, Action<TextWriter, object> partialBlockTemplate, IDictionary<string, Action<TextWriter, object>> inlinePartialTemplates) :
            this(value, writer, parent, templatePath, partialBlockTemplate, null, inlinePartialTemplates) { }

        public BindingContext(object value, EncodedTextWriter writer, BindingContext parent, string templatePath, Action<TextWriter, object> partialBlockTemplate, BindingContext current, IDictionary<string, Action<TextWriter, object>> inlinePartialTemplates)
        {
            RegisterValueProvider(new BindingContextValueProvider(this));
            
            TemplatePath = parent != null ? (parent.TemplatePath ?? templatePath) : templatePath;
            TextWriter = writer;
            Value = value;
            ParentContext = parent;

            //Inline partials cannot use the Handlebars.RegisteredTemplate method
            //because it pollutes the static dictionary and creates collisions
            //where the same partial name might exist in multiple templates.
            //To avoid collisions, pass around a dictionary of compiled partials
            //in the context
            if (parent != null)
            {
                InlinePartialTemplates = parent.InlinePartialTemplates;

                if (value is HashParameterDictionary dictionary) {
                    // Populate value with parent context
                    foreach (var item in GetContextDictionary(parent.Value)) {
                        if (!dictionary.ContainsKey(item.Key))
                            dictionary[item.Key] = item.Value;
                    }
                }
            }
            else if (current != null)
            {
                InlinePartialTemplates = current.InlinePartialTemplates;
            }
            else if (inlinePartialTemplates != null)
            {
                InlinePartialTemplates = inlinePartialTemplates;
            }
            else
            {
                InlinePartialTemplates = new Dictionary<string, Action<TextWriter, object>>(StringComparer.OrdinalIgnoreCase);
            }

            PartialBlockTemplate = partialBlockTemplate;
        }

        public string TemplatePath { get; }

        public EncodedTextWriter TextWriter { get; }

        public IDictionary<string, Action<TextWriter, object>> InlinePartialTemplates { get; }

        public Action<TextWriter, object> PartialBlockTemplate { get; }

        public bool SuppressEncoding
        {
            get => TextWriter.SuppressEncoding;
            set => TextWriter.SuppressEncoding = value;
        }
        
        public virtual object Value { get; }

        public virtual BindingContext ParentContext { get; }

        public virtual object Root => ParentContext?.Root ?? this;

        public void RegisterValueProvider(IValueProvider valueProvider)
        {
            if(valueProvider == null) throw new ArgumentNullException(nameof(valueProvider));
            
            _valueProviders.Add(valueProvider);
        }

        public virtual object GetContextVariable(string variableName)
        {
            // accessing value providers in reverse order as it gives more probability of hit
            for (var index = _valueProviders.Count - 1; index >= 0; index--)
            {
                if (_valueProviders[index].TryGetValue(variableName, out var value)) return value;
            }

            return null;
        }
        
        public virtual object GetVariable(string variableName)
        {
            // accessing value providers in reverse order as it gives more probability of hit
            for (var index = _valueProviders.Count - 1; index >= 0; index--)
            {
                var valueProvider = _valueProviders[index];
                if(!valueProvider.ProvidesNonContextVariables) continue;
                
                if (valueProvider.TryGetValue(variableName, out var value)) return value;
            }

            return ParentContext?.GetVariable(variableName);
        }

        private static IDictionary<string, object> GetContextDictionary(object target)
        {
            var dict = new Dictionary<string, object>();

            if (target == null)
                return dict;

            if (target is IDictionary<string, object> dictionary) {
                foreach (var item in dictionary)
                    dict[item.Key] = item.Value;
            } else {
                var type = target.GetType();

                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var field in fields) {
                    dict[field.Name] = field.GetValue(target);
                }

                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties) {
                    if (property.GetIndexParameters().Length == 0)
                        dict[property.Name] = property.GetValue(target);
                }
            }

            return dict;
        }

        public virtual BindingContext CreateChildContext(object value, Action<TextWriter, object> partialBlockTemplate)
        {
            return new BindingContext(value ?? Value, TextWriter, this, TemplatePath, partialBlockTemplate ?? PartialBlockTemplate, null);
        }
    }
}
