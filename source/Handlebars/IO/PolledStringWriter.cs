using System;
using System.Globalization;
using System.IO;
using HandlebarsDotNet.Pools;

namespace HandlebarsDotNet
{
    public class ReusableStringWriter : StringWriter
    {
        private static readonly InternalObjectPool<ReusableStringWriter, Policy> Pool = new InternalObjectPool<ReusableStringWriter, Policy>(new Policy(16));
        
        private IFormatProvider _formatProvider;

        public static ReusableStringWriter Get(IFormatProvider formatProvider = null)
        {
            var writer = Pool.Get();
            writer._formatProvider = formatProvider ?? CultureInfo.CurrentCulture;
            return writer;
        }
        
        private ReusableStringWriter()
        {
        }

        public override IFormatProvider FormatProvider => _formatProvider;

        protected override void Dispose(bool disposing)
        {
            _formatProvider = null;
            Pool.Return(this);
        }

        private readonly struct Policy : IInternalObjectPoolPolicy<ReusableStringWriter>
        {
            private readonly StringBuilderPool.StringBuilderPooledObjectPolicy _policy;

            public Policy(int initialCapacity, int maximumRetainedCapacity = 4096)
            {
                _policy = new StringBuilderPool.StringBuilderPooledObjectPolicy(initialCapacity, maximumRetainedCapacity);
            }

            public ReusableStringWriter Create() => new ReusableStringWriter();

            public bool Return(ReusableStringWriter item) => _policy.Return(item.GetStringBuilder());
        }
    }
}