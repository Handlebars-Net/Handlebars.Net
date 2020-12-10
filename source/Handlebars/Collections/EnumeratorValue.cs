using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.Collections
{
    public readonly ref struct EnumeratorValue<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EnumeratorValue(T value, int index, bool isLast)
        {
            Value = value;
            Index = index;
            IsLast = isLast;
            IsFirst = index == 0;
        }

        public readonly T Value;
        public readonly int Index;
        public readonly bool IsFirst;
        public readonly bool IsLast;
    }
}