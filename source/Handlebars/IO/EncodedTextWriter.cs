using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using HandlebarsDotNet.IO;

namespace HandlebarsDotNet
{
	public readonly struct EncodedTextWriter : IDisposable
	{
		private readonly IFormatterProvider _formatterProvider;
		private readonly TextEncoderWrapper _encoder;

		internal readonly TextWriter UnderlyingWriter;

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
			IFormatterProvider formatterProvider, 
			bool suppressEncoding = false)
		{
			UnderlyingWriter = writer;
			_formatterProvider = formatterProvider;
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
				_encoder.Encode(value, UnderlyingWriter);
				return;
			}
			
			UnderlyingWriter.Write(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(StringBuilder value, bool encode = true)
		{
			if(encode && !SuppressEncoding)
			{
				_encoder.Encode(value, UnderlyingWriter);
				return;
			}

			for (int i = 0; i < value.Length; i++)
			{
				UnderlyingWriter.Write(value[i]);
			}
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write<T>(T value, bool encode) where T: IEnumerator<char>
		{
			if(encode && !SuppressEncoding)
			{
				_encoder.Encode(value, UnderlyingWriter);
				return;
			}

			while (value.MoveNext())
			{
				UnderlyingWriter.Write(value.Current);
			}
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(string value) => Write(value, true);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(string format, params object[] arguments) => Write(string.Format(format, arguments), true);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(char value) => Write(value.SequenceOfOne(), true);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(object value) => Write<object>(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write<T>(T value)
		{
			switch (value)
			{
				case null:
				case string v when string.IsNullOrEmpty(v): 
				case StringBuilder st when st.Length == 0:
					return;
				
				case string v: Write(v, true); return;
				case StringBuilder v: Write(v, true); return;
				
				default:
					WriteFormatted(value);
					return;
			}
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void WriteFormatted<T>(T value)
		{
			var type = typeof(T);
#if netstandard1_3
			if (type.GetTypeInfo().IsClass) type = value.GetType();
#else
			if (type.IsClass) type = value.GetType();
#endif

			if (!_formatterProvider.TryCreateFormatter(type, out var formatter))
				Throw.CannotResolveFormatter(type);

			formatter.Format(value, this);
		}

		public Encoding Encoding
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => UnderlyingWriter.Encoding;
		}
		
		public void Dispose() => _encoder.Dispose();
		
		public override string ToString() => UnderlyingWriter.ToString();
		
		private static class Throw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static void CannotResolveFormatter(Type type) => throw new HandlebarsRuntimeException($"Cannot resolve formatter for type `{type}`");
		}
	}
}