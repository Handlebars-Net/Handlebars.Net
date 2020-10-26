using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using HandlebarsDotNet.Compiler;

namespace HandlebarsDotNet
{
    internal sealed class EncodedTextWriterWrapper : TextWriter
    {
        private static readonly EncodedTextWriterPool Pool = new EncodedTextWriterPool();
		
        public EncodedTextWriter UnderlyingWriter { get; private set; }
		
        public static TextWriter From(EncodedTextWriter encodedTextWriter)
        {
            var textWriter = Pool.Get();
            textWriter.UnderlyingWriter = encodedTextWriter;
			
            return textWriter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(string value, bool encode) => UnderlyingWriter.Write(value, encode);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(StringBuilder value, bool encode) => UnderlyingWriter.Write(value, encode);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(UndefinedBindingResult undefined) => UnderlyingWriter.Write(undefined);
        
        public override void Write(string value) => UnderlyingWriter.Write(value);

        public override void Write(char value) => UnderlyingWriter.Write(value);
		
        public override void Write(int value) => UnderlyingWriter.Write(value);
		
        public override void Write(double value) => UnderlyingWriter.Write(value);
		
        public override void Write(float value) => UnderlyingWriter.Write(value);

        public override void Write(decimal value) => UnderlyingWriter.Write(value);
		
        public override void Write(bool value) => UnderlyingWriter.Write(value);
		
        public override void Write(long value) => UnderlyingWriter.Write(value);
		
        public override void Write(ulong value) => UnderlyingWriter.Write(value);
		
        public override void Write(uint value) => UnderlyingWriter.Write(value);

        public override void Write(object value)
        {
            if (value is StringBuilder builder)
            {
                UnderlyingWriter.Write(builder);
                return;
            }

            UnderlyingWriter.Write(value);
        }

        public override Encoding Encoding => UnderlyingWriter.Encoding;

        protected override void Dispose(bool disposing) => Pool.Return(this);

        private class EncodedTextWriterPool : InternalObjectPool<EncodedTextWriterWrapper>
        {
            public EncodedTextWriterPool() : base(new Policy())
            {
            }
			
            private class Policy : IInternalObjectPoolPolicy<EncodedTextWriterWrapper>
            {
                public EncodedTextWriterWrapper Create()
                {
                    return new EncodedTextWriterWrapper();
                }
		
                public bool Return(EncodedTextWriterWrapper obj)
                {
                    obj.UnderlyingWriter = default;
					
                    return true;
                }
            }
        }
    }
}