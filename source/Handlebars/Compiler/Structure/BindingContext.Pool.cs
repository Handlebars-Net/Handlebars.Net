using System;
using System.IO;

namespace HandlebarsDotNet.Compiler
{
    public sealed partial class BindingContext
    {
        private static readonly BindingContextPool Pool = new BindingContextPool();

        internal static BindingContext Create(ICompiledHandlebarsConfiguration configuration, object value,
            EncodedTextWriter writer, BindingContext parent, string templatePath)
        {
            return Pool.CreateContext(configuration, value, writer, parent, templatePath, null);
        }

        internal static BindingContext Create(ICompiledHandlebarsConfiguration configuration, object value,
            EncodedTextWriter writer, BindingContext parent, string templatePath,
            Action<BindingContext, TextWriter, object> partialBlockTemplate)
        {
            return Pool.CreateContext(configuration, value, writer, parent, templatePath, partialBlockTemplate);
        }
        
        public void Dispose() => Pool.Return(this);
        
        private class BindingContextPool : InternalObjectPool<BindingContext>
        {
            public BindingContextPool() : base(new BindingContextPolicy())
            {
            }
            
            public BindingContext CreateContext(ICompiledHandlebarsConfiguration configuration, object value, EncodedTextWriter writer, BindingContext parent, string templatePath, Action<BindingContext, TextWriter, object> partialBlockTemplate)
            {
                var context = Get();
                context.Configuration = configuration;
                context.Value = value;
                context.TextWriter = writer;
                context.ParentContext = parent;
                context.TemplatePath = templatePath;
                context.PartialBlockTemplate = partialBlockTemplate;

                context.Initialize();

                return context;
            }
        
            private class BindingContextPolicy : IInternalObjectPoolPolicy<BindingContext>
            {
                public BindingContext Create() => new BindingContext();

                public bool Return(BindingContext item)
                {
                    item.Root = null;
                    item.Value = null;
                    item.ParentContext = null;
                    item.TemplatePath = null;
                    item.TextWriter = null;
                    item.PartialBlockTemplate = null;
                    item.InlinePartialTemplates.Clear();

                    item.BlockParamsObject.OptionalClear();
                    item.ContextDataObject.OptionalClear();
                    
                    item._objectDescriptor.Reset();

                    return true;
                }
            }
        }
    }
}