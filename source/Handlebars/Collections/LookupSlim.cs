using System;
using System.Collections.Generic;
using System.Threading;

namespace HandlebarsDotNet.Collections
{
    internal sealed class LookupSlim<TKey, TValue, TComparer> : IIndexed<TKey, TValue>
        where TComparer : IEqualityComparer<TKey>
    {
        private DictionarySlim<TKey, TValue, TComparer> _inner;

        public LookupSlim(TComparer comparer)
        {
            _inner = new DictionarySlim<TKey, TValue, TComparer>(comparer);
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

        public bool TryGetValue(in TKey key, out TValue value) => _inner.TryGetValue(key, out value);

        public void Clear() => _inner.Clear();

        private TValue Write(TKey key, TValue value)
        {
            var inner = _inner;
            var copy = new DictionarySlim<TKey, TValue, TComparer>(inner);
            copy.AddOrReplace(key, value);
            
            Interlocked.CompareExchange(ref _inner, copy, inner);

            return value;
        }
    }
}