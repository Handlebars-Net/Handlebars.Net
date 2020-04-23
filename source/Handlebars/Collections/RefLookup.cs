using System.Collections.Generic;

namespace HandlebarsDotNet.Collections
{
    internal sealed class RefLookup<TKey, TValue> 
        where TValue: struct
    {
        private readonly RefDictionary<TKey, TValue> _inner;

        public delegate ref TValue ValueFactory(TKey key, ref TValue value);
        
        public RefLookup(int capacity = 16, IEqualityComparer<TKey> comparer = null)
        {
            _inner = new RefDictionary<TKey, TValue>(capacity, comparer);
        }

        public bool ContainsKey(TKey key)
        {
            return _inner.ContainsKey(key);
        }

        public ref TValue GetValueOrDefault(TKey key)
        {
            return ref _inner[key];
        }

        public ref TValue GetOrAdd(TKey key, ValueFactory factory)
        {
            if (_inner.ContainsKey(key))
            {
                return ref _inner[key];
            }

            return ref factory(key, ref _inner[key]);
        }
    }
}