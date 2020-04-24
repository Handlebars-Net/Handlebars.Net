using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HandlebarsDotNet.Collections
{
    internal class CascadeDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _outer;
        private readonly IDictionary<TKey, TValue> _inner;

        public CascadeDictionary(IDictionary<TKey, TValue> outer, IEqualityComparer<TKey> comparer = null)
        {
            _outer = outer;
            _inner = new Dictionary<TKey, TValue>(comparer);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var value in _outer)
            {
                if (_inner.TryGetValue(value.Key, out var innerValue))
                {
                    yield return new KeyValuePair<TKey, TValue>(value.Key, innerValue);
                }
                else
                {
                    yield return value;   
                }
            }
            
            foreach (var value in _inner)
            {
                if (_outer.ContainsKey(value.Key)) continue;
                
                yield return value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _inner.Add(item);
        }

        public void Clear()
        {
            _inner.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _inner.Contains(item) || _outer.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var value in _outer)
            {
                if (_inner.TryGetValue(value.Key, out var innerValue))
                {
                    array[arrayIndex] = new KeyValuePair<TKey, TValue>(value.Key, innerValue);
                    arrayIndex++;
                }
                else
                {
                    array[arrayIndex] = value;
                    arrayIndex++;
                }
            }
            
            foreach (var value in _inner)
            {
                if (_outer.ContainsKey(value.Key)) continue;
                
                array[arrayIndex] = value;
                arrayIndex++;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _inner.Remove(item);
        }

        public int Count
        {
            get
            {
                var count = _outer.Count;
                foreach (var value in _inner)
                {
                    if (_outer.ContainsKey(value.Key)) continue;
                    count++;
                }

                return count;
            }
        }

        public bool IsReadOnly => _inner.IsReadOnly;

        public bool ContainsKey(TKey key)
        {
            return _inner.ContainsKey(key) || _outer.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            _inner.Add(key, value);
        }

        public bool Remove(TKey key)
        {
            return _inner.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _inner.TryGetValue(key, out value) || _outer.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => !_inner.TryGetValue(key, out var value) ? _outer[key] : value;
            set => _inner[key] = value;
        }

        public ICollection<TKey> Keys => this.Select(o => o.Key).ToArray();

        public ICollection<TValue> Values => this.Select(o => o.Value).ToArray();
    }
}