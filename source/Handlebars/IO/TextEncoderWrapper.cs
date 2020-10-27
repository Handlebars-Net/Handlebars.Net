using System;
using System.IO;
using System.Text;

namespace HandlebarsDotNet
{
    internal class TextEncoderWrapper : ITextEncoder, IDisposable
    {
        private static readonly InternalObjectPool<TextEncoderWrapper> Pool 
            = new InternalObjectPool<TextEncoderWrapper>(new Policy());

        private ITextEncoder _underlyingEncoder;
        private bool _enabled;

        public static TextEncoderWrapper Null { get; } = new TextEncoderWrapper();
			
        public static TextEncoderWrapper Create(ITextEncoder encoder)
        {
            var wrapper = Pool.Get();
            wrapper._underlyingEncoder = encoder;
            wrapper.Enabled = encoder != null;

            return wrapper;
        }

        public bool Enabled
        {
            get => _enabled && _underlyingEncoder != null;
            set => _enabled = value;
        }

        private TextEncoderWrapper()
        {
        }

        public void Encode(StringBuilder text, TextWriter target)
        {
            if (!Enabled) return;

            _underlyingEncoder.Encode(text, target);
        }

        public void Encode(string text, TextWriter target)
        {
            if (!Enabled) return;

            _underlyingEncoder.Encode(text, target);
        }

        public bool ShouldEncode(char c)
        {
            return Enabled && _underlyingEncoder.ShouldEncode(c);
        }

        public IFormatProvider FormatProvider => _underlyingEncoder.FormatProvider;

        public void Dispose()
        {
            if(ReferenceEquals(this, Null)) return;
            
            Pool.Return(this);
        }

        private class Policy : IInternalObjectPoolPolicy<TextEncoderWrapper>
        {
            public TextEncoderWrapper Create() => new TextEncoderWrapper();

            public bool Return(TextEncoderWrapper item)
            {
                item._enabled = true;
                item._underlyingEncoder = null;

                return true;
            }
        }
    }
}