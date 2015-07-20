using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace HandlebarsDotNet
{
	internal class EncodedTextWriter : TextWriter
	{
		private readonly TextWriter _underlyingWriter;

		public EncodedTextWriter(TextWriter writer)
		{
			_underlyingWriter = writer;
		}

		public override void Write(string value)
		{
			_underlyingWriter.Write(HtmlEncode(value));
		}

		public override void Write(char value)
		{
			_underlyingWriter.Write(HtmlEncode(value.ToString()));
		}

		public override void Write(object value)
		{
			if (value == null)
			{
				return;
			}
			if (value is ISafeString)
			{
				_underlyingWriter.Write(value.ToString());
			}
			else
			{
				this.Write(value.ToString());
			}
		}

		public TextWriter UnderlyingWriter
		{
			get { return _underlyingWriter; }
		}

		public override Encoding Encoding
		{
			get { return _underlyingWriter.Encoding; }
		}
		private static string HtmlEncode(string text)
		{
			if (text == null)
				return String.Empty;

			var sb = new StringBuilder(text.Length);

			for (var i = 0; i < text.Length; i++)
			{
				switch (text[i])
				{
					case '"':
						sb.Append("&quot;");
						break;
					case '&':
						sb.Append("&amp;");
						break;
					case '<':
						sb.Append("&lt;");
						break;
					case '>':
						sb.Append("&gt;");
						break;
					
					default:
						if (text[i] > 159)
						{
							sb.Append("&#");
							sb.Append(((int)text[i]).ToString(CultureInfo.InvariantCulture));
							sb.Append(";");
						}
						else
							sb.Append(text[i]);
						break;
				}
			}
			return sb.ToString();
		}
	}


}

