using System.Runtime.CompilerServices;
using System.Text;

namespace HandlebarsDotNet.StringUtils
{
    internal readonly struct StringBuilderWrapper : IStringWrapper
    {
        private readonly StringBuilder _value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringBuilderWrapper(StringBuilder value) => _value = value;
        
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _value.Length;
        }

        public char this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _value[index];
        }
    }
}