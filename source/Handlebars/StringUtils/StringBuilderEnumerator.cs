using System.Runtime.CompilerServices;
using System.Text;

namespace HandlebarsDotNet.StringUtils
{
    internal ref struct StringBuilderEnumerator
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
        public bool MoveNext()
        {
            if (++_index >= _stringBuilder.Length) return false;
            
            Current = _stringBuilder[_index];
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => _index = -1;

        public char Current { get; private set; }
    }
}