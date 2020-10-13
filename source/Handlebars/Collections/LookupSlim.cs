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
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
            _inner = new Dictionary<TKey, TValue>(_comparer);
        }

        public bool ContainsKey(TKey key) => _inner.ContainsKey(key);

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            return !_inner.TryGetValue(key, out var value) 
                ? Write(key, valueFactory(key)) 
                : value;
        }
        
        public TValue GetOrAdd<TState>(TKey key, Func<TKey, TState, TValue> valueFactory, TState state)
        {
            return !_inner.TryGetValue(key, out var value) 
                ? Write(key, valueFactory(key, state)) 
                : value;
        }

        public bool TryGetValue(TKey key, out TValue value) => _inner.TryGetValue(key, out value);

        public void Clear() => _inner.Clear();

        private TValue Write(TKey key, TValue value)
        {
            var inner = _inner;
            var copy = new Dictionary<TKey, TValue>(inner, _comparer)
            {
                [key] = value
            };
            
            Interlocked.CompareExchange(ref _inner, copy, inner);

            return value;
        }
    }
}