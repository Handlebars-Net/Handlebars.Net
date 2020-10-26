using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using HandlebarsDotNet.Compiler;

namespace HandlebarsDotNet
{
	public readonly struct EncodedTextWriter : IDisposable
	{
		private readonly TextWriter _underlyingWriter;
		private readonly Func<UndefinedBindingResult, string> _undefinedFormatter;

		private readonly TextEncoderWrapper _encoder;
		
		public bool SuppressEncoding
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => !_encoder.Enabled;
			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => _encoder.Enabled = !value;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public EncodedTextWriter(
			TextWriter writer, 
			ITextEncoder encoder, 
			Func<UndefinedBindingResult, string> undefinedFormatter, 
			bool suppressEncoding = false)
		{
			_underlyingWriter = writer;
			_undefinedFormatter = undefinedFormatter;

			_encoder = encoder != null 
				? TextEncoderWrapper.Create(encoder) 
				: TextEncoderWrapper.Null;
			
			SuppressEncoding = suppressEncoding;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TextWriter CreateWrapper() => EncodedTextWriterWrapper.From(this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(string value, bool encode)
		{
			if(encode && !SuppressEncoding)
			{
				_encoder.Encode(value, _underlyingWriter);
				return;
			}
			
			_underlyingWriter.Write(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(StringBuilder value, bool encode = true)
		{
			if(encode && !SuppressEncoding)
			{
				_encoder.Encode(value, _underlyingWriter);
				return;
			}

			for (int i = 0; i < value.Length; i++)
			{
				_underlyingWriter.Write(value[i]);
			}
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(string value)
		{
			if(!SuppressEncoding)
			{
				_encoder.Encode(value, _underlyingWriter);
				return;
			}

			_underlyingWriter.Write(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(string format, params object[] arguments) => Write(string.Format(format, arguments));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(char value)
		{
			if (_encoder.ShouldEncode(value))
			{
				Write(value.ToString(), true);
			}
			else
			{
				_underlyingWriter.Write(value);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(UndefinedBindingResult undefined) => _underlyingWriter.Write(_undefinedFormatter(undefined));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(int value) => _underlyingWriter.Write(value);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(double value) => _underlyingWriter.Write(value);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(float value) => _underlyingWriter.Write(value);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(bool value) => _underlyingWriter.Write(value);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(decimal value) => _underlyingWriter.Write(value);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(short value) => _underlyingWriter.Write(value);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(long value) => _underlyingWriter.Write(value);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(ulong value) => _underlyingWriter.Write(value);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(uint value) => _underlyingWriter.Write(value);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(ushort value) => _underlyingWriter.Write(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(object value)
		{
			switch (value)
			{
				case string v: Write(v); return;
				case UndefinedBindingResult v: Write(v); return;
				case bool v: Write(v); return;
				case int v: Write(v); return;
				case char v: Write(v); return;
				case float v: Write(v); return;
				case double v: Write(v); return;
				case long v: Write(v); return;
				case short v: Write(v); return;
				case uint v: Write(v); return;
				case ulong v: Write(v); return;
				case ushort v: Write(v); return;
				case decimal v: Write(v); return;
				
				default:
					var @string = value.ToString();
					if(string.IsNullOrEmpty(@string)) return;
			
					Write(@string);
					return;
			}
		}

		public Encoding Encoding
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _underlyingWriter.Encoding;
		}
		
		public void Dispose() => _encoder.Dispose();
		
		public override string ToString() => _underlyingWriter.ToString();
	}
}