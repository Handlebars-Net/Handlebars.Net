using System;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Pools;

namespace HandlebarsDotNet
{
    public sealed partial class BindingContext
    {
        private static readonly BindingContextPool Pool = new BindingContextPool();

        internal static BindingContext Create(ICompiledHandlebarsConfiguration configuration, object? value)
        {
            return Pool.CreateContext(configuration, value, null, null);
        }

        internal static BindingContext Create(ICompiledHandlebarsConfiguration configuration, object? value, BindingContext parent,
            TemplateDelegate? partialBlockTemplate)
        {
            return Pool.CreateContext(configuration, value, parent, partialBlockTemplate);
        }
        
        public void Dispose() => Pool.Return(this);
        
        private class BindingContextPool : InternalObjectPool<BindingContext, BindingContextPool.BindingContextPolicy>
        {
            public BindingContextPool() : base(new BindingContextPolicy())
            {
            }
            
            public BindingContext CreateContext(ICompiledHandlebarsConfiguration configuration, object? value, BindingContext? parent, TemplateDelegate? partialBlockTemplate)
            {
                var context = Get();
                context.Configuration = configuration;
                context.Value = value;
                context.ParentContext = parent;
                context.PartialBlockTemplate = partialBlockTemplate;
                context.PartialDepth = parent?.PartialDepth ?? 0;

                context.Initialize();

                return context;
            }

            internal struct BindingContextPolicy : IInternalObjectPoolPolicy<BindingContext>
            {
                BindingContext IInternalObjectPoolPolicy<BindingContext>.Create() => new BindingContext();

                public bool Return(BindingContext item)
                {
                    item.Configuration = null!;

                    item.HasFrameHelpers = false;
                    item.Root = null!;
                    item.Value = null!;
                    item.ParentContext = null;
                    item.PartialBlockTemplate = null;
                    item.InlinePartialTemplates.Clear();
                    item.Helpers.Clear();
                    item.BlockHelpers.Clear();
                    item.Bag.Clear();

                    item.BlockParamsObject.OptionalClear();
                    item.ContextDataObject.OptionalClear();
                    // Indexes into ContextDataObject from this use must not survive into the next one:
                    // once the dictionary's byte version wraps they would look current again.
                    Array.Clear(item.WellKnownVariables, 0, item.WellKnownVariables.Length);

                    item.Descriptor.Reset();

                    return true;
                }
            }
        }
    }
}