using System.IO;
using System.Text;

namespace HandlebarsDotNet
{
	internal class EncodedTextWriter : TextWriter
	{
		private readonly ITextEncoder _encoder;

		public bool SuppressEncoding { get; set; }

		private EncodedTextWriter(TextWriter writer, ITextEncoder encoder)
		{
			UnderlyingWriter = writer;
			_encoder = encoder;
		}

		public static EncodedTextWriter From(TextWriter writer, ITextEncoder encoder)
		{
			var encodedTextWriter = writer as EncodedTextWriter;

			return encodedTextWriter ?? new EncodedTextWriter(writer, encoder);
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

			var encode = !(value is ISafeString);
			Write(value.ToString(), encode);
		}

		public TextWriter UnderlyingWriter { get; }

		public override Encoding Encoding => UnderlyingWriter.Encoding;
	}
}