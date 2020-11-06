using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.Collections
{
    internal class CascadeIndex<TKey, TValue, TComparer> : IIndexed<TKey, TValue> 
        where TComparer : IEqualityComparer<TKey>
    {
        public IIndexed<TKey, TValue> Outer { get; set; }
        private readonly DictionarySlim<TKey, TValue, TComparer> _inner;

        public CascadeIndex(TComparer comparer)
        {
            Outer = null;
            _inner = new DictionarySlim<TKey, TValue, TComparer>(comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in TKey key, TValue value)
        {
            _inner.AddOrReplace(key, value);
        }

        public void Clear()
        {
            Outer = null;
            _inner.Clear();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(in TKey key, out TValue value)
        {
            if (_inner.TryGetValue(key, out value)) return true;
            return Outer?.TryGetValue(key, out value) ?? false;
        }
    }
}