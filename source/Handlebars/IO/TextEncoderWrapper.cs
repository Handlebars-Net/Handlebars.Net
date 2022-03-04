using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HandlebarsDotNet.Pools;

namespace HandlebarsDotNet
{
    internal class TextEncoderWrapper : ITextEncoder, IDisposable
    {
        private static readonly InternalObjectPool<TextEncoderWrapper, Policy> Pool 
            = new InternalObjectPool<TextEncoderWrapper, Policy>(new Policy());

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

        public void Encode<T>(T text, TextWriter target) where T: IEnumerator<char>
        {
            if (!Enabled) return;

            _underlyingEncoder.Encode(text, target);
        }

        public void Dispose()
        {
            if(ReferenceEquals(this, Null)) return;
            
            Pool.Return(this);
        }

        private struct Policy : IInternalObjectPoolPolicy<TextEncoderWrapper>
        {
            public TextEncoderWrapper Create() => new TextEncoderWrapper();
            public bool TryClaim(TextEncoderWrapper item) => true;
            public bool Return(TextEncoderWrapper item)
            {
                item._enabled = true;
                item._underlyingEncoder = null;

                return true;
            }
        }
    }
}