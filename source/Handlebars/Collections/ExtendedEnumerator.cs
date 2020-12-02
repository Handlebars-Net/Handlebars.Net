using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.Collections
{
    public ref struct ExtendedEnumerator<T>
    {
        private readonly IEnumerator _enumerator;

        private T _next;
        private int _index;
        private bool _hasNext;

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
            if (!_hasNext) return false;
                    
            PerformIteration();

            return true;
        }

        private void PerformIteration()
        {
            if (!_enumerator.MoveNext())
            {
                Current = _hasNext
                    ? new EnumeratorValue<T>(_next, _index++, true)
                    : new EnumeratorValue<T>();

                _hasNext = false;
                _next = default;
                return;
            }
            
            if (!_hasNext)
            {
                _hasNext = true;
                _next = (T) _enumerator.Current;
                return;
            }

            Current = new EnumeratorValue<T>(_next, _index++, false);
            _next = (T) _enumerator.Current;
        }
    }
    
    public ref struct ExtendedEnumerator<T, TEnumerator>
        where TEnumerator: IEnumerator<T>
    {
        private TEnumerator _enumerator;

        private T _next;
        private int _index;
        private bool _hasNext;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ExtendedEnumerator(TEnumerator enumerator) : this()
        {
            _enumerator = enumerator;
            PerformIteration();
        }
            
        public EnumeratorValue<T> Current { get; private set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (!_hasNext) return false;
                    
            PerformIteration();

            return true;
        }

        private void PerformIteration()
        {
            if (!_enumerator.MoveNext())
            {
                Current = _hasNext
                    ? new EnumeratorValue<T>(_next, _index++, true)
                    : new EnumeratorValue<T>();

                _hasNext = false;
                _next = default;
                return;
            }
            
            if (!_hasNext)
            {
                _hasNext = true;
                _next = _enumerator.Current;
                return;
            }

            Current = new EnumeratorValue<T>(_next, _index++, false);
            _next = _enumerator.Current;
        }
    }
}