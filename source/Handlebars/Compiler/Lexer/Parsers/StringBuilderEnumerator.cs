using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class StringBuilderEnumerator : IEnumerable<char>
    {
        private readonly StringBuilder _stringBuilder;

        public StringBuilderEnumerator(StringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;
        }

        public IEnumerator<char> GetEnumerator()
        {
            for (int index = 0; index < _stringBuilder.Length; index++)
            {
                yield return _stringBuilder[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}