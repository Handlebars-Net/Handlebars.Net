using System;
using System.Collections.Generic;
using System.IO;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Contains properties accessible withing <see cref="HandlebarsBlockHelper"/> function 
    /// </summary>
    public sealed class HelperOptions : IDisposable
    {
        private static readonly InternalObjectPool<HelperOptions> Pool = new InternalObjectPool<HelperOptions>(new Policy());
        
        private readonly Dictionary<string, object> _extensions;

        internal static HelperOptions Create(Action<BindingContext, TextWriter, object> template,
            Action<BindingContext, TextWriter, object> inverse,
            ChainSegment[] blockParamsValues,
            BindingContext bindingContext)
        {
            var item = Pool.Get();

            item.OriginalTemplate = template;
            item.OriginalInverse = inverse;
            
            item.BindingContext = bindingContext;
            item.Configuration = bindingContext.Configuration;
            item.BlockParams = blockParamsValues;
            item.Data = new DataValues(bindingContext);

            return item;
        }
        
        private HelperOptions()
        {
            _extensions = new Dictionary<string, object>(7);
            Template = (writer, o) => OriginalTemplate(BindingContext, writer, o);
            Inverse = (writer, o) => OriginalInverse(BindingContext, writer, o);
        }

        /// <summary>
        /// BlockHelper body
        /// </summary>
        public Action<TextWriter, object> Template { get; }

        /// <summary>
        /// BlockHelper <c>else</c> body
        /// </summary>
        public Action<TextWriter, object> Inverse { get; }

        public ChainSegment[] BlockParams { get; private set; }

        public DataValues Data { get; private set; }

        internal ICompiledHandlebarsConfiguration Configuration { get; private set; }
        internal BindingContext BindingContext { get; private set; }
        internal Action<BindingContext, TextWriter, object> OriginalTemplate { get; private set; }
        internal Action<BindingContext, TextWriter, object> OriginalInverse { get; private set; }
        
        public BindingContext CreateFrame(object value = null)
        {
            return BindingContext.CreateFrame(value);
        }
        
        /// <summary>
        /// Provides access to dynamic options
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
        public T GetValue<T>(string property) => (T) this[property];

        public void Dispose() => Pool.Return(this);

        private class Policy : IInternalObjectPoolPolicy<HelperOptions>
        {
            public HelperOptions Create() => new HelperOptions();

            public bool Return(HelperOptions item)
            {
                item._extensions.Clear();

                item.Configuration = null;
                item.BindingContext = null;
                item.BlockParams = null;
                item.OriginalInverse = null;
                item.OriginalTemplate = null;

                return true;
            }
        }
    }
}

