using HandlebarsDotNet.Compiler.Structure.Path;
using Microsoft.Extensions.ObjectPool;

namespace HandlebarsDotNet.ValueProviders
{
    internal class ObjectEnumeratorValueProvider : IteratorValueProvider
    {
        private HandlebarsConfiguration _configuration;

        private static readonly ObjectEnumeratorValueProviderPool Pool = new ObjectEnumeratorValueProviderPool();

        public static ObjectEnumeratorValueProvider Create(HandlebarsConfiguration configuration)
        {
            var provider = Pool.Get();
            provider._configuration = configuration;
            return provider;
        }

        public string Key { get; set; }

        public override bool TryGetValue(ref ChainSegment segment, out object value)
        {
            switch (segment.LowerInvariant)
            {
                case "key":
                    value = Key;
                    return true;

                case "last" when !_configuration.Compatibility.SupportLastInObjectIterations:
                    value = null;
                    return true;

                default:
                    return base.TryGetValue(ref segment, out value);
            }
        }
        
        public override void Dispose()
        {
            Pool.Return(this);
        }
        
        private class ObjectEnumeratorValueProviderPool : DefaultObjectPool<ObjectEnumeratorValueProvider>
        {
            public ObjectEnumeratorValueProviderPool() : base(new ObjectEnumeratorValueProviderPolicy())
            {
            }
            
            private class ObjectEnumeratorValueProviderPolicy : IPooledObjectPolicy<ObjectEnumeratorValueProvider>
            {
                ObjectEnumeratorValueProvider IPooledObjectPolicy<ObjectEnumeratorValueProvider>.Create()
                {
                    return new ObjectEnumeratorValueProvider();
                }

                bool IPooledObjectPolicy<ObjectEnumeratorValueProvider>.Return(ObjectEnumeratorValueProvider item)
                {
                    item.First = true;
                    item.Last = false;
                    item.Index = 0;
                    item.Value = null;
                    item.Key = null;
                    item._configuration = null;

                    return true;
                }
            }
        }
    }
}