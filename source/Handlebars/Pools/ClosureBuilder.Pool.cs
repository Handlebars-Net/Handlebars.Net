using System;
using HandlebarsDotNet.Pools;

namespace HandlebarsDotNet.Compiler
{
    public partial class ClosureBuilder : IDisposable
    {
        private static readonly ClosureBuilderPool Pool = new (new Policy());
        
        private ClosureBuilder() { }

        public static ClosureBuilder Create() => Pool.Get();
        
        public void Dispose()
        {
            _pathInfos.Clear();
            _templateDelegates.Clear();
            _decoratorDelegates.Clear();
            _blockParams.Clear();
            _helpers.Clear();
            _blockHelpers.Clear();
            _decorators.Clear();
            _blockDecorators.Clear();
            _other.Clear();
            
            Pool.Return(this);
        }

        private sealed class ClosureBuilderPool : InternalObjectPool<ClosureBuilder, Policy>
        {
            public ClosureBuilderPool(Policy policy) : base(policy)
            {
            }
        }
        
        private readonly struct Policy : IInternalObjectPoolPolicy<ClosureBuilder>
        {
            public ClosureBuilder Create() => new ();
            public bool Return(ClosureBuilder item) => true;
        }
    }
}