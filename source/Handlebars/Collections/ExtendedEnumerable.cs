using System.Collections;
using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.Collections
{
    internal ref struct ExtendedEnumerator<T>
    {
        private readonly IEnumerator _enumerator;
        private T _next;
        private int _index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ExtendedEnumerator(IEnumerator enumerator) : this()
        {
            _enumerator = enumerator;
            PerformIteration();
        }
            
        public EnumeratorValue<T> Current { get; private set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (_next == null) return false;
                    
            PerformIteration();

            return true;
        }

        private void PerformIteration()
        {
            if (!_enumerator.MoveNext())
            {
                Current = _next != null
                    ? new EnumeratorValue<T>(_next, _index++, true)
                    : EnumeratorValue<T>.Empty;

                _next = default;
                return;
            }

            if (_next == null)
            {
                _next = (T) _enumerator.Current;
                return;
            }

            Current = new EnumeratorValue<T>(_next, _index++, false);
            _next = (T) _enumerator.Current;
        }
    }
    
    internal readonly struct EnumeratorValue<T>
    {
        public static readonly EnumeratorValue<T> Empty = new EnumeratorValue<T>();
        
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