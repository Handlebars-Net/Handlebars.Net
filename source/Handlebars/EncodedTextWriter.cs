using System.IO;
using System.Text;

namespace HandlebarsDotNet
{
	internal class EncodedTextWriter : TextWriter
	{
		private readonly TextWriter _underlyingWriter;
		private readonly ITextEncoder _encoder;

		public EncodedTextWriter(TextWriter writer, ITextEncoder encoder)
		{
			_underlyingWriter = writer;
			_encoder = encoder;
		}

		public void Write(string value, bool encode)
		{
			if (encode && (_encoder != null))
			{
				value = _encoder.Encode(value);
			}

			_underlyingWriter.Write(value);
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

			var encode = !(value is ISafeString);
			Write(value.ToString(), encode);
		}

		public TextWriter UnderlyingWriter
		{
			get { return _underlyingWriter; }
		}

		public override Encoding Encoding
		{
			get { return _underlyingWriter.Encoding; }
		}
	}
}