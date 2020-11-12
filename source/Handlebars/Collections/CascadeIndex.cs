using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.Collections
{
    /// <summary>
    /// Allows to create chains of <see cref="IIndexed{TKey,TValue}"/> collections
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TComparer"></typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public class CascadeIndex<TKey, TValue, TComparer> : IIndexed<TKey, TValue> 
        where TComparer : IEqualityComparer<TKey>
    {
        public IReadOnlyIndexed<TKey, TValue> Outer { get; set; }
        private readonly DictionarySlim<TKey, TValue, TComparer> _inner;

        public CascadeIndex(TComparer comparer)
        {
            Outer = null;
            _inner = new DictionarySlim<TKey, TValue, TComparer>(comparer);
        }
        
        public int Count => _inner.Count + OuterEnumerable().Count();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddOrReplace(in TKey key, in TValue value)
        {
            _inner.AddOrReplace(key, value);
        }

        public void Clear()
        {
            Outer = null;
            _inner.Clear();
        }

        public bool ContainsKey(in TKey key)
        {
            return _inner.ContainsKey(key) || (Outer?.ContainsKey(key) ?? false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(in TKey key, out TValue value)
        {
            if (_inner.TryGetValue(key, out value)) return true;
            return Outer?.TryGetValue(key, out value) ?? false;
        }

        public TValue this[in TKey key]
        {
            get
            {
                if (_inner.TryGetValue(key, out var value)) return value;
                if (Outer?.TryGetValue(key, out value) ?? false) return value;
                throw new KeyNotFoundException($"{key}");
            }

            set => _inner.AddOrReplace(key, value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var enumerator = _inner.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }

            foreach (var pair in OuterEnumerable())
            {
                yield return pair;
            }
        }

        private IEnumerable<KeyValuePair<TKey, TValue>> OuterEnumerable()
        {
            var outerEnumerator = Outer?.GetEnumerator();
            if (outerEnumerator == null) yield break;

            while (outerEnumerator.MoveNext())
            {
                if (_inner.ContainsKey(outerEnumerator.Current.Key)) continue;
                yield return outerEnumerator.Current;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}