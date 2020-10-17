using System;
using System.Globalization;
using System.IO;

namespace HandlebarsDotNet
{
    internal class ReusableStringWriter : StringWriter
    {
        private static readonly InternalObjectPool<ReusableStringWriter> Pool = new InternalObjectPool<ReusableStringWriter>(new Policy());
        
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

        private class Policy : IInternalObjectPoolPolicy<ReusableStringWriter>
        {
            private readonly StringBuilderPool.StringBuilderPooledObjectPolicy _policy = new StringBuilderPool.StringBuilderPooledObjectPolicy();
                
            public ReusableStringWriter Create()
            {
                return new ReusableStringWriter();
            }

            public bool Return(ReusableStringWriter item)
            {
                return _policy.Return(item.GetStringBuilder());
            }
        }
    }
}