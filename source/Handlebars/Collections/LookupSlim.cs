using System;
using System.Collections.Generic;
using System.Threading;

namespace HandlebarsDotNet.Collections
{
    internal sealed class LookupSlim<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _inner;
        private readonly IEqualityComparer<TKey> _comparer;

        public LookupSlim(IEqualityComparer<TKey> comparer = null)
        {
            _inner = new Dictionary<TKey, TValue>();
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        public bool ContainsKey(TKey key)
        {
            return _inner.ContainsKey(key);
        }
        
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            return !_inner.TryGetValue(key, out var value) 
                ? Write(key, valueFactory(key)) 
                : value;
        }

        private TValue Write(TKey key, TValue value)
        {
            var copy = new Dictionary<TKey, TValue>(_inner, _comparer)
            {
                [key] = value
            };
            
            Interlocked.CompareExchange(ref _inner, copy, _inner);

            return value;
        }
    }
}