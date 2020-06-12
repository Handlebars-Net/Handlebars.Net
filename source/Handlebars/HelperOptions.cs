using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HandlebarsDotNet.Compiler;

namespace HandlebarsDotNet
{
    /// <summary>
    /// 
    /// </summary>
    public delegate void BlockParamsConfiguration(ConfigureBlockParams blockParamsConfiguration, params object[] dependencies);
    
    /// <summary>
    /// Contains properties accessible withing <see cref="HandlebarsBlockHelper"/> function 
    /// </summary>
    public sealed class HelperOptions : IReadOnlyDictionary<string, object>
    {
        private readonly Dictionary<string, object> _extensions;
        
        internal HelperOptions(
            Action<TextWriter, object> template,
            Action<TextWriter, object> inverse,
            BlockParamsValueProvider blockParamsValueProvider,
            InternalHandlebarsConfiguration configuration,
            BindingContext bindingContext)
        {
            Template = template;
            Inverse = inverse;
            BlockParams = blockParamsValueProvider.Configure;
            
            BindingContext = bindingContext;
            Configuration = configuration;
            
            _extensions = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(Template)] = Template,
                [nameof(Inverse)] = Inverse,
                [nameof(BlockParams)] = BlockParams
            };
        }

        /// <summary>
        /// BlockHelper body
        /// </summary>
        public Action<TextWriter, object> Template { get; }

        /// <summary>
        /// BlockHelper <c>else</c> body
        /// </summary>
        public Action<TextWriter, object> Inverse { get; }

        /// <inheritdoc cref="ConfigureBlockParams"/>
        public BlockParamsConfiguration BlockParams { get; }
        
        /// <inheritdoc cref="HandlebarsConfiguration"/>
        internal InternalHandlebarsConfiguration Configuration { get; }
        
        internal BindingContext BindingContext { get; }

        bool IReadOnlyDictionary<string, object>.ContainsKey(string key)
        {
            return _extensions.ContainsKey(key);
        }

        bool IReadOnlyDictionary<string, object>.TryGetValue(string key, out object value)
        {
            return _extensions.TryGetValue(key, out value);
        }

        /// <summary>
        /// Provides access to dynamic data entries
        /// </summary>
        /// <param name="property"></param>
        public object this[string property]
        {
            get => _extensions.TryGetValue(property, out var value) ? value : null;
            internal set => _extensions[property] = value;
        }

        /// <summary>
        /// Provides access to dynamic data entries in a typed manner
        /// </summary>
        /// <param name="property"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetValue<T>(string property)
        {
            return (T) this[property];
        }

        IEnumerable<string> IReadOnlyDictionary<string, object>.Keys => _extensions.Keys;

        IEnumerable<object> IReadOnlyDictionary<string, object>.Values => _extensions.Values;

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return _extensions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _extensions).GetEnumerator();
        }

        int IReadOnlyCollection<KeyValuePair<string, object>>.Count => _extensions.Count;
    }
}

