using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal struct StringBuilderEnumerator : IEnumerator<char>
    {
        private readonly StringBuilder _stringBuilder;
        private int _index;

        public StringBuilderEnumerator(StringBuilder stringBuilder) : this()
        {
            _stringBuilder = stringBuilder;
            _index = -1;
        }

        public bool MoveNext()
        {
            if (++_index >= _stringBuilder.Length) return false;
            
            Current = _stringBuilder[_index];
            return true;
        }

        public void Reset() => _index = -1;

        public char Current { get; private set; }

        object IEnumerator.Current => Current;
        
        public void Dispose()
        {
        }
    }
}