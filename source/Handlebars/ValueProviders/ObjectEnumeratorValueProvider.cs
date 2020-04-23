using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.ValueProviders
{
    internal class ObjectEnumeratorValueProvider : IteratorValueProvider
    {
        private HandlebarsConfiguration _configuration;

        private static readonly ObjectEnumeratorValueProviderPool Pool = new ObjectEnumeratorValueProviderPool();

        public static ObjectEnumeratorValueProvider Create(HandlebarsConfiguration configuration)
        {
            var provider = Pool.GetObject();
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
        
        private class ObjectEnumeratorValueProviderPool : ObjectPool<ObjectEnumeratorValueProvider>
        {
            protected override ObjectEnumeratorValueProvider CreateObject()
            {
                return new ObjectEnumeratorValueProvider();
            }

            public override void PutObject(ObjectEnumeratorValueProvider item)
            {
                item.First = false;
                item.Last = false;
                item.Index = 0;
                item.Value = null;
                item.Key = null;
                item._configuration = null;

                base.PutObject(item);
            }
        }
    }
}