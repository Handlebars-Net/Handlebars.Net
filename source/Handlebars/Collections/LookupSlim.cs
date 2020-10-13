using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace HandlebarsDotNet.Collections
{
    internal sealed class LookupSlim<TKey, TValue> where TKey : class
    {
        private static readonly DictionaryPool<TKey, TValue> Pool = DictionaryPool<TKey, TValue>.Shared;

        private Dictionary<TKey, TValue> _inner;

        public LookupSlim() => _inner = Pool.Get();

        public bool ContainsKey(TKey key) => _inner.ContainsKey(key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            return _inner.TryGetValue(key, out var value) 
                ? value 
                : Write(key, valueFactory(key));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue GetOrAdd<TState>(TKey key, Func<TKey, TState, TValue> valueFactory, TState state)
        {
            return _inner.TryGetValue(key, out var value)
                ? value 
                : Write(key, valueFactory(key, state));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(TKey key, out TValue value) => _inner.TryGetValue(key, out value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => _inner.Clear();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TValue Write(TKey key, TValue value)
        {
            var copy = Pool.Get();
            var inner = _inner;
            inner.CopyTo(copy);
            copy[key] = value;

            if (Interlocked.CompareExchange(ref _inner, copy, inner) != _inner)
            {
                Pool.Return(inner);
            }
            else
            {
                Pool.Return(copy);   
            }

            return value;
        }
    }
}