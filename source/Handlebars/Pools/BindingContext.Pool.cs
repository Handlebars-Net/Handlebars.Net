using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Pools;

namespace HandlebarsDotNet
{
    public sealed partial class BindingContext
    {
        private static readonly BindingContextPool Pool = new BindingContextPool();

        internal static BindingContext Create(ICompiledHandlebarsConfiguration configuration, object value, string templatePath = null)
        {
            return Pool.CreateContext(configuration, value, null, templatePath, null);
        }

        internal static BindingContext Create(ICompiledHandlebarsConfiguration configuration, object value, BindingContext parent, string templatePath,
            TemplateDelegate partialBlockTemplate)
        {
            return Pool.CreateContext(configuration, value, parent, templatePath, partialBlockTemplate);
        }
        
        public void Dispose() => Pool.Return(this);
        
        private class BindingContextPool : InternalObjectPool<BindingContext, BindingContextPool.BindingContextPolicy>
        {
            public BindingContextPool() : base(new BindingContextPolicy())
            {
            }
            
            public BindingContext CreateContext(ICompiledHandlebarsConfiguration configuration, object value, BindingContext parent, string templatePath, TemplateDelegate partialBlockTemplate)
            {
                var context = Get();
                context.Configuration = configuration;
                context.Value = value;
                context.ParentContext = parent;
                context.TemplatePath = templatePath;
                context.PartialBlockTemplate = partialBlockTemplate;

                context.Initialize();

                return context;
            }

            internal struct BindingContextPolicy : IInternalObjectPoolPolicy<BindingContext>
            {
                public BindingContext Create() => new BindingContext();

                public bool Return(BindingContext item)
                {
                    item.Root = null;
                    item.Value = null;
                    item.ParentContext = null;
                    item.TemplatePath = null;
                    item.PartialBlockTemplate = null;
                    item.InlinePartialTemplates.Clear();

                    item.RootDataObject.Clear();
                    item.Extensions.OptionalClear();
                    item.BlockParamsObject.OptionalClear();
                    item.ContextDataObject.OptionalClear();
                    
                    item._objectDescriptor.Reset();

                    return true;
                }
            }
        }
    }
}