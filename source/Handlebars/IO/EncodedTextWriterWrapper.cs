using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using HandlebarsDotNet.Pools;

namespace HandlebarsDotNet
{
    internal sealed class EncodedTextWriterWrapper : TextWriter
    {
        private static readonly InternalObjectPool<EncodedTextWriterWrapper, Policy> Pool = new InternalObjectPool<EncodedTextWriterWrapper, Policy>(new Policy());
		
        public EncodedTextWriter UnderlyingWriter { get; private set; }
		
        public static TextWriter From(in EncodedTextWriter encodedTextWriter)
        {
            var textWriter = Pool.Get();
            textWriter.UnderlyingWriter = encodedTextWriter;
			
            return textWriter;
        }

        public override IFormatProvider FormatProvider => UnderlyingWriter.Encoder.FormatProvider;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(string value, bool encode) => UnderlyingWriter.Write(value, encode);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(StringBuilder value, bool encode) => UnderlyingWriter.Write(value, encode);
        
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
        
        private readonly struct Policy : IInternalObjectPoolPolicy<EncodedTextWriterWrapper>
        {
	        public EncodedTextWriterWrapper Create() => new EncodedTextWriterWrapper();

	        public bool Return(EncodedTextWriterWrapper obj)
	        {
		        obj.UnderlyingWriter = default;
		        return true;
	        }
        }
    }
}