using System.Collections;
using System.Collections.Generic;

namespace HandlebarsDotNet.Collections
{
    // Will be removed in next iterations
    internal class CascadeCollection<T> : ICollection<T>
    {
        private readonly ICollection<T> _outer;
        private readonly ICollection<T> _inner = new List<T>();

        public CascadeCollection(ICollection<T> outer)
        {
            _outer = outer;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var value in _outer)
            {
                yield return value;
            }
            
            foreach (var value in _inner)
            {
                yield return value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _inner.Add(item);
        }

        public void Clear()
        {
            _inner.Clear();
        }

        public bool Contains(T item)
        {
            return _inner.Contains(item) || _outer.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var value in this)
            {
                array[arrayIndex] = value;
                arrayIndex++;
            }
        }

        public bool Remove(T item)
        {
            return _inner.Remove(item);
        }

        public int Count => _outer.Count + _inner.Count;

        public bool IsReadOnly => _inner.IsReadOnly;
    }
}