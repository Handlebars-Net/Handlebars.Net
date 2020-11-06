using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace HandlebarsDotNet
{
    public readonly struct Formatter<T>
    {
        private readonly object _state;
        private readonly Action<T, object, TextWriter> _formatter;
		    
        public Formatter(object state, Action<T, object, TextWriter> formatter)
        {
            _state = state;
            _formatter = formatter;
        }
        
        public Formatter(Action<T, TextWriter> formatter)
        {
            _state = formatter;
            _formatter = (v, s, w) => ((Action<T, TextWriter>) s)(v, w);
        }
		
        public Formatter(Func<T, string> formatter)
        {
            _state = formatter;
            _formatter = (v, s, w) => w.Write(((Func<T, string>) s)(v));
        }
        
        public void Format(T item, TextWriter writer) => _formatter(item, _state, writer);

        [Pure]
        public string Format(T item)
        {
            using var writer = ReusableStringWriter.Get();
            _formatter(item, _state, writer);
            return writer.ToString();
        }

        public static implicit operator Formatter<T>(string format) =>
            new Formatter<T>(format, (value, state, w) =>
            {
                w.Write((string) state, value);
            });

        public static implicit operator Formatter<T>(Func<T, string> format) => new Formatter<T>(format);
        public static implicit operator Formatter<T>(Action<T, TextWriter> format) => new Formatter<T>(format);
    }
}