using System.Collections;

namespace HandlebarsDotNet.Collections
{
    /// <summary>
    /// Wraps <see cref="IEnumerable"/> and provide additional information about the iteration via <see cref="EnumeratorValue{T}"/>
    /// </summary>
    internal sealed class ExtendedEnumerable<T>
    {
        private Enumerator _enumerator;

        public ExtendedEnumerable(IEnumerable enumerable)
        {
            _enumerator = new Enumerator(enumerable.GetEnumerator());
        }
            
        public ref Enumerator GetEnumerator()
        {
            return ref _enumerator;
        }

        internal struct Enumerator
        {
            private readonly IEnumerator _enumerator;
            private Container<T> _next;
            private int _index;

            public Enumerator(IEnumerator enumerator) : this()
            {
                _enumerator = enumerator;
                PerformIteration();
            }
            
            public EnumeratorValue<T> Current { get; private set; }

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
                        ? new EnumeratorValue<T>(_next.Value, _index++, true)
                        : EnumeratorValue<T>.Empty;

                    _next = null;
                    return;
                }

                if (_next == null)
                {
                    _next = new Container<T>((T) _enumerator.Current);
                    return;
                }

                Current = new EnumeratorValue<T>(_next.Value, _index++, false);
                _next.Value = (T) _enumerator.Current;
            }
        }
        
        private class Container<TValue>
        {
            public TValue Value { get; set; }

            public Container(TValue value)
            {
                Value = value;
            }
        }
    }

    internal struct EnumeratorValue<T>
    {
        public static readonly EnumeratorValue<T> Empty = new EnumeratorValue<T>();
        
        public EnumeratorValue(T value, int index, bool isLast)
        {
            Value = value;
            Index = index;
            IsLast = isLast;
            IsFirst = index == 0;
        }

        public T Value { get; }
        public int Index { get; }
        public bool IsFirst { get; }
        public bool IsLast { get; }
    }
}