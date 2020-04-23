using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.ValueProviders
{
    internal class IteratorValueProvider : IValueProvider
    {
        private static readonly IteratorValueProviderPool Pool = new IteratorValueProviderPool();

        public static IteratorValueProvider Create()
        {
            return Pool.GetObject();
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
        
        private class IteratorValueProviderPool : ObjectPool<IteratorValueProvider>
        {
            protected override IteratorValueProvider CreateObject()
            {
                return new IteratorValueProvider();
            }

            public override void PutObject(IteratorValueProvider item)
            {
                item.First = false;
                item.Last = false;
                item.Index = 0;
                item.Value = null;

                base.PutObject(item);
            }
        }

        public void Dispose()
        {
            Pool.PutObject(this);
        }
    }
}