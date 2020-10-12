using HandlebarsDotNet.Compiler.Structure.Path;
using Microsoft.Extensions.ObjectPool;

namespace HandlebarsDotNet.ValueProviders
{
    internal class IteratorValueProvider : IValueProvider
    {
        private static readonly IteratorValueProviderPool Pool = new IteratorValueProviderPool();

        public static IteratorValueProvider Create()
        {
            return Pool.Get();
        }
        
        public object Value { get; set; }

        public int Index { get; set; }

        public bool First { get; set; }

        public bool Last { get; set; }

        public ValueTypes SupportedValueTypes { get; } = ValueTypes.Context;

        public virtual bool TryGetValue(ref ChainSegment segment, out object value)
        {
            switch (segment.LowerInvariant)
            {
                case "index":
                    value = Index;
                    return true;
                case "first":
                    value = First;
                    return true;
                case "last":
                    value = Last;
                    return true;
                case "value":
                    value = Value;
                    return true;

                default:
                    value = null;
                    return false;
            }
        }
        
        public virtual void Dispose()
        {
            Pool.Return(this);
        }
        
        private class IteratorValueProviderPool : DefaultObjectPool<IteratorValueProvider>
        {
            public IteratorValueProviderPool() : base(new IteratorValueProviderPolicy())
            {
            }
            
            private class IteratorValueProviderPolicy : IPooledObjectPolicy<IteratorValueProvider>
            {
                IteratorValueProvider IPooledObjectPolicy<IteratorValueProvider>.Create()
                {
                    return new IteratorValueProvider();
                }

                bool IPooledObjectPolicy<IteratorValueProvider>.Return(IteratorValueProvider item)
                {
                    item.First = true;
                    item.Last = false;
                    item.Index = 0;
                    item.Value = null;

                    return true;
                }
            }
        }
    }
}