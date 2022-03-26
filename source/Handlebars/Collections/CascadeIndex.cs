using System;
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
        private readonly TComparer _comparer;
        public IReadOnlyIndexed<TKey, TValue> Outer { get; set; }
        private DictionarySlim<TKey, TValue, TComparer> _inner;

        public CascadeIndex(TComparer comparer)
            : this(null, comparer)
        {
        }
        
        public CascadeIndex(IReadOnlyIndexed<TKey, TValue> outer, TComparer comparer)
        {
            _comparer = comparer;
            Outer = outer;
        }
        
        public int Count => (_inner?.Count ?? 0) + OuterEnumerable().Count();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddOrReplace(in TKey key, in TValue value)
        {
            (_inner ??= new DictionarySlim<TKey, TValue, TComparer>(_comparer)).AddOrReplace(key, value);
        }

        public void Clear()
        {
            Outer = null;
            _inner?.Clear();
        }

        public bool ContainsKey(in TKey key)
        {
            return (_inner?.ContainsKey(key) ?? false) 
                   || (Outer?.ContainsKey(key) ?? false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(in TKey key, out TValue value)
        {
            value = default;
            return (_inner?.TryGetValue(key, out value) ?? false) 
                   || (Outer?.TryGetValue(key, out value) ?? false);
        }

        public TValue this[in TKey key]
        {
            get
            {
                if (TryGetValue(key, out var value)) return value;
                Throw.KeyNotFoundException($"{key}");
                return default; // will never reach this point
            }

            set => AddOrReplace(key, value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var pair in InnerEnumerable()) yield return pair;
            foreach (var pair in OuterEnumerable()) yield return pair;
        }

        private IEnumerable<KeyValuePair<TKey, TValue>> InnerEnumerable()
        {
            if(_inner == null) yield break; 
            
            var outerEnumerator = _inner.GetEnumerator();
            while (outerEnumerator.MoveNext())
            {
                if (_inner.ContainsKey(outerEnumerator.Current.Key)) continue;
                yield return outerEnumerator.Current;
            }
        }
        
        private IEnumerable<KeyValuePair<TKey, TValue>> OuterEnumerable()
        {
            if(Outer == null) yield break;
            
            using var outerEnumerator = Outer.GetEnumerator();
            while (outerEnumerator.MoveNext())
            {
                if (_inner.ContainsKey(outerEnumerator.Current.Key)) continue;
                yield return outerEnumerator.Current;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        private static class Throw
        {
            public static void KeyNotFoundException(string message, Exception exception = null) => throw new KeyNotFoundException(message, exception);
        }
    }
}