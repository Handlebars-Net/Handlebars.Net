using System;
using System.IO;

namespace HandlebarsDotNet
{
    internal class PolledStringWriter : StringWriter
    {
        public PolledStringWriter() : base(StringBuilderPool.Shared.Get())
        {
            
        }
        
        public PolledStringWriter(IFormatProvider formatProvider) : base(StringBuilderPool.Shared.Get(), formatProvider)
        {
        }

        protected override void Dispose(bool disposing)
        {
            StringBuilderPool.Shared.Return(base.GetStringBuilder());
            base.Dispose(disposing);
        }
    }
}