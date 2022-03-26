using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.IO;
using HandlebarsDotNet.ObjectDescriptors;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.Pools;

namespace HandlebarsDotNet.Runtime
{
    public sealed class AmbientContext : IDisposable
    {
        private static readonly InternalObjectPool<AmbientContext, Policy> Pool = new(new Policy()); 
        
        [ThreadStatic]
        private static Stack<AmbientContext> _local;

        private static Stack<AmbientContext> Local => 
            _local ??= new Stack<AmbientContext>();

        public static AmbientContext Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Local.Count > 0 ? Local.Peek() : null;
        }

        public static AmbientContext Create(
            PathInfoStore pathInfoStore = null, 
            ChainSegmentStore chainSegmentStore = null, 
            UndefinedBindingResultCache undefinedBindingResultCache = null,
            FormatterProvider formatterProvider = null,
            ObjectDescriptorFactory descriptorFactory = null
        )
        {
            var ambientContext = Pool.Get();
            
            ambientContext.PathInfoStore = pathInfoStore ?? new PathInfoStore();
            ambientContext.ChainSegmentStore = chainSegmentStore ?? new ChainSegmentStore();
            ambientContext.UndefinedBindingResultCache = undefinedBindingResultCache ?? new UndefinedBindingResultCache();
            ambientContext.FormatterProvider = formatterProvider ?? new FormatterProvider();
            ambientContext.ObjectDescriptorFactory = descriptorFactory ?? new ObjectDescriptorFactory();

            return ambientContext;
        }

        public static AmbientContext Create(
            AmbientContext context,
            PathInfoStore pathInfoStore = null, 
            ChainSegmentStore chainSegmentStore = null, 
            UndefinedBindingResultCache undefinedBindingResultCache = null,
            FormatterProvider formatterProvider = null,
            ObjectDescriptorFactory descriptorFactory = null
        )
        {
            var ambientContext = Pool.Get();
            
            ambientContext.PathInfoStore = pathInfoStore ?? context.PathInfoStore;
            ambientContext.ChainSegmentStore = chainSegmentStore ?? context.ChainSegmentStore;
            ambientContext.UndefinedBindingResultCache = undefinedBindingResultCache ?? context.UndefinedBindingResultCache;
            ambientContext.FormatterProvider = (formatterProvider ?? new FormatterProvider()).Append(context.FormatterProvider);
            ambientContext.ObjectDescriptorFactory = (descriptorFactory ?? new ObjectDescriptorFactory()).Append(context.ObjectDescriptorFactory);

            return ambientContext;
        }

        public static DisposableContainer Use(AmbientContext ambientContext)
        {
            Local.Push(ambientContext);
            
            return new DisposableContainer(() => Local.Pop());
        }
        
        private AmbientContext()
        {
        }
        
        public PathInfoStore PathInfoStore { get; private set; }
        
        public ChainSegmentStore ChainSegmentStore { get; private set; }
        
        public UndefinedBindingResultCache UndefinedBindingResultCache { get; private set; }
        
        public FormatterProvider FormatterProvider { get; private set; }
        
        public ObjectDescriptorFactory ObjectDescriptorFactory { get; private set; }
        
        public Dictionary<string, object> Bag { get; } = new();
        
        private struct Policy : IInternalObjectPoolPolicy<AmbientContext>
        {
            public AmbientContext Create() => new();

            public bool Return(AmbientContext item)
            {
                item.PathInfoStore = null;
                item.ChainSegmentStore = null;
                item.UndefinedBindingResultCache = null;
                item.FormatterProvider = null;
                item.ObjectDescriptorFactory = null;
                item.Bag.Clear();

                return true;
            }
        }

        public void Dispose() => Pool.Return(this);
    }
}