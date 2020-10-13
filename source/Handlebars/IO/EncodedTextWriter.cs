using System.IO;
using System.Text;

namespace HandlebarsDotNet
{
	internal sealed class EncodedTextWriter : TextWriter
	{
		private static readonly EncodedTextWriterPool Pool = new EncodedTextWriterPool();
		
		private ITextEncoder _encoder;

		public bool SuppressEncoding { get; set; }

		private EncodedTextWriter()
		{
		}

		public static EncodedTextWriter From(TextWriter writer, ITextEncoder encoder)
		{
			if (writer is EncodedTextWriter encodedTextWriter) return encodedTextWriter;
			
			var textWriter = Pool.Get();
			textWriter._encoder = encoder;
			textWriter.UnderlyingWriter = writer;

			return textWriter;
		}

		public void Write(string value, bool encode)
		{
			if(encode && !SuppressEncoding && (_encoder != null))
			{
				value = _encoder.Encode(value);
			}

			UnderlyingWriter.Write(value);
		}

		public override void Write(string value)
		{
			Write(value, true);
		}

		public override void Write(char value)
		{
			Write(value.ToString(), true);
		}

		public override void Write(object value)
		{
			if (value == null)
			{
				return;
			}

			if (value is ISafeString safeString)
			{
				Write(safeString.Value, false);
				return;
			}
			
			var @string = value as string ?? value.ToString();
			if(string.IsNullOrEmpty(@string)) return;
			
			Write(@string, true);
		}

		public TextWriter UnderlyingWriter { get; private set; }

		protected override void Dispose(bool disposing)
		{
			Pool.Return(this);
		}

		public override Encoding Encoding => UnderlyingWriter.Encoding;
		
		private class EncodedTextWriterPool : InternalObjectPool<EncodedTextWriter>
		{
			public EncodedTextWriterPool() : base(new Policy())
			{
			}
			
			private class Policy : IInternalObjectPoolPolicy<EncodedTextWriter>
			{
				public EncodedTextWriter Create()
				{
					return new EncodedTextWriter();
				}

				public bool Return(EncodedTextWriter obj)
				{
					obj._encoder = null;
					obj.UnderlyingWriter = null;
					
					return true;
				}
			}
		}
	}
}