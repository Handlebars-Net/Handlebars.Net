using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.StringUtils
{
    internal readonly struct StringWrapper : IStringWrapper
    {
        private readonly string _value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringWrapper(string value) => _value = value;
        
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