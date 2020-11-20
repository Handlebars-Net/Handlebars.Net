using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace HandlebarsDotNet.StringUtils
{
    internal struct StringBuilderEnumerator : IEnumerator<char>
    {
        private readonly StringBuilder _stringBuilder;
        private int _index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringBuilderEnumerator(StringBuilder stringBuilder) : this()
        {
            _stringBuilder = stringBuilder;
            _index = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => ++_index < _stringBuilder.Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => _index = -1;

        object IEnumerator.Current => Current;

        public char Current => _stringBuilder[_index];
        
        public void Dispose()
        {
            // nothing to do here
        }
    }
}