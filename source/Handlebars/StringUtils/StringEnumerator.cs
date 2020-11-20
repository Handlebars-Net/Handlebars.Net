using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.StringUtils
{
    internal struct StringEnumerator : IEnumerator<char>
    {
        private readonly string _text;
        private readonly int _length;
        
        private int _index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringEnumerator(string text)
        {
            _text = text;
            _length = _text.Length;
            _index = -1;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => ++_index < _length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => _index = -1;

        public char Current => _text[_index];

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            // nothing to do here
        }
    }
}