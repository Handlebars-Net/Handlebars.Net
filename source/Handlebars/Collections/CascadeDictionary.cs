using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HandlebarsDotNet.Collections
{
    internal class CascadeDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private static readonly Dictionary<TKey, TValue> Empty = new Dictionary<TKey, TValue>();

        public IDictionary<TKey, TValue> Outer { get; set; }
        private readonly Dictionary<TKey, TValue> _inner;
        
        public CascadeDictionary(IEqualityComparer<TKey> comparer = null)
        {
            Outer = Empty;
            _inner = new Dictionary<TKey, TValue>(comparer ?? EqualityComparer<TKey>.Default);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var value in _inner)
            {
                yield return value;
            }
            
            foreach (var value in Outer)
            {
                if (_inner.ContainsKey(value.Key)) continue;
                
                yield return value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _inner.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            Outer = Empty;
            _inner.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => _inner.Contains(item) || Outer.Contains(item);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var value in Outer)
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
                if (Outer.ContainsKey(value.Key)) continue;

                array[arrayIndex] = value;
                arrayIndex++;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) => _inner.Remove(item.Key);

        public int Count
        {
            get
            {
                var count = Outer.Count;
                foreach (var value in _inner)
                {
                    if (Outer.ContainsKey(value.Key)) continue;
                    count++;
                }

                return count;
            }
        }

        public bool IsReadOnly => false;

        public bool ContainsKey(TKey key) => _inner.ContainsKey(key) || Outer.ContainsKey(key);

        public void Add(TKey key, TValue value) => _inner.Add(key, value);

        public bool Remove(TKey key) => _inner.Remove(key);

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _inner.TryGetValue(key, out value) || Outer.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => !_inner.TryGetValue(key, out var value) ? Outer[key] : value;
            set => _inner[key] = value;
        }

        public ICollection<TKey> Keys => this.Select(o => o.Key).ToArray();

        public ICollection<TValue> Values => this.Select(o => o.Value).ToArray();
    }
}