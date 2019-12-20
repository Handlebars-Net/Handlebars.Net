using System;
using System.Collections;
using System.Collections.Generic;

namespace HandlebarsDotNet.Compiler
{
    /// <summary>
    /// Wraps <see cref="IEnumerable"/> and provide additional information about the iteration via <see cref="EnumeratorValue{T}"/>
    /// </summary>
    internal sealed class ExtendedEnumerable<T> : IEnumerable<EnumeratorValue<T>>
    {
        private readonly IEnumerable _enumerable;

        public ExtendedEnumerable(IEnumerable enumerable)
        {
            _enumerable = enumerable;
        }
            
        public IEnumerator<EnumeratorValue<T>> GetEnumerator()
        {
            return new Enumerator(_enumerable.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private sealed class Enumerator : IEnumerator<EnumeratorValue<T>>
        {
            private readonly IEnumerator _enumerator;
            private Container<T> _next;
            private int _index;

            public Enumerator(IEnumerator enumerator)
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

            public void Reset()
            {
                _next = null;
                Current = null;
                _enumerator.Reset();
                PerformIteration();
            }

            object IEnumerator.Current => Current;
                
            public void Dispose()
            {
                (_enumerator as IDisposable)?.Dispose();
            }
            
            private void PerformIteration()
            {
                if (!_enumerator.MoveNext())
                {
                    Current = _next != null
                        ? new EnumeratorValue<T>(_next.Value, _index++, true)
                        : null;

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
            
            private class Container<TValue>
            {
                public TValue Value { get; set; }

                public Container(TValue value)
                {
                    Value = value;
                }
            }
        }
    }
    
    internal class EnumeratorValue<T>
    {
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