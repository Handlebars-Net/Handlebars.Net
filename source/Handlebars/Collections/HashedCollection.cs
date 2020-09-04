using System;
using System.Collections;
using System.Collections.Generic;

namespace HandlebarsDotNet.Collections
{
    internal class HashedCollection<T> : IReadOnlyList<T>, ICollection<T> where T:class
    {
        private readonly List<T> _list = new List<T>();
        private readonly Dictionary<T, int> _mapping = new Dictionary<T, int>();
        
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                yield return _list[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            if(item == null) throw new ArgumentNullException(nameof(item));
            
            if(_mapping.ContainsKey(item)) return;
            _mapping.Add(item, _list.Count);
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
            _mapping.Clear();
        }

        public bool Contains(T item)
        {
            if(item == null) throw new ArgumentNullException(nameof(item));
            
            return _mapping.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if(item == null) throw new ArgumentNullException(nameof(item));
            
            if (!_mapping.TryGetValue(item, out var index)) return false;
            _list.RemoveAt(index);
            _mapping.Remove(item);
            return true;
        }

        public int Count => _list.Count;
        public bool IsReadOnly { get; } = false;

        public T this[int index] => _list[index];
    }
}