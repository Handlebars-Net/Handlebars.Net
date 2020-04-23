using System;
using System.IO;

namespace HandlebarsDotNet
{
    internal class PolledStringWriter : StringWriter
    {
        public PolledStringWriter() : base(StringBuilderPool.Shared.GetObject())
        {
            
        }
        
        public PolledStringWriter(IFormatProvider formatProvider) : base(StringBuilderPool.Shared.GetObject(), formatProvider)
        {
        }

        protected override void Dispose(bool disposing)
        {
            StringBuilderPool.Shared.PutObject(base.GetStringBuilder());
            base.Dispose(disposing);
        }
    }
}