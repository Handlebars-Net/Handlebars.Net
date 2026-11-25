using System;
using System.Globalization;
using System.IO;
using HandlebarsDotNet.Pools;

namespace HandlebarsDotNet
{
    // Sealed: the only constructor is private, so no external subclass could ever exist.
    // Dispose(bool) intentionally does not call base.Dispose: the instance is returned to
    // the pool for reuse rather than closed.
    public sealed class ReusableStringWriter : StringWriter
    {
        // Retain up to 32K chars (64KB, below the LOH threshold) so that templates producing
        // typical page-sized output keep their grown StringBuilder across renders. The previous
        // 4096-char limit made any render larger than 4KB discard the writer, forcing every
        // subsequent render to re-grow a fresh StringBuilder(16) chunk by chunk.
        private static readonly InternalObjectPool<ReusableStringWriter, Policy> Pool = new InternalObjectPool<ReusableStringWriter, Policy>(new Policy(16, 32 * 1024));
        
        private IFormatProvider _formatProvider = null!;

        public static ReusableStringWriter Get(IFormatProvider? formatProvider = null)
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
            _formatProvider = null!;
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