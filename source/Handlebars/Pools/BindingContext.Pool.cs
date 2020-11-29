using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Pools;

namespace HandlebarsDotNet
{
    public sealed partial class BindingContext
    {
        private static readonly BindingContextPool Pool = new BindingContextPool();

        internal static BindingContext Create(ICompiledHandlebarsConfiguration configuration, object value)
        {
            return Pool.CreateContext(configuration, value, null, null);
        }

        internal static BindingContext Create(ICompiledHandlebarsConfiguration configuration, object value, BindingContext parent,
            TemplateDelegate partialBlockTemplate)
        {
            return Pool.CreateContext(configuration, value, parent, partialBlockTemplate);
        }
        
        public void Dispose() => Pool.Return(this);
        
        private class BindingContextPool : InternalObjectPool<BindingContext, BindingContextPool.BindingContextPolicy>
        {
            public BindingContextPool() : base(new BindingContextPolicy())
            {
            }
            
            public BindingContext CreateContext(ICompiledHandlebarsConfiguration configuration, object value, BindingContext parent, TemplateDelegate partialBlockTemplate)
            {
                var context = Get();
                context.Configuration = configuration;
                context.Value = value;
                context.ParentContext = parent;
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
                    item.PartialBlockTemplate = null;
                    item.InlinePartialTemplates.Clear();
                    item.Bag.Clear();

                    item.RootDataObject.Clear();
                    item.BlockParamsObject.OptionalClear();
                    item.ContextDataObject.OptionalClear();
                    
                    item.Descriptor.Reset();

                    return true;
                }
            }
        }
    }
}