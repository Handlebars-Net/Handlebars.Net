using System.Collections.Generic;
using System.Threading;

namespace HandlebarsDotNet.Collections
{
    internal sealed class HashSetSlim<TKey>
    {
        private HashSet<TKey> _inner;
        private readonly IEqualityComparer<TKey> _comparer;

        public HashSetSlim(IEqualityComparer<TKey> comparer = null)
        {
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
            _inner = new HashSet<TKey>(_comparer);
        }

        public bool Contains(TKey key)
        {
            return _inner.Contains(key);
        }
        
        public void Add(TKey key)
        {
            var copy = new HashSet<TKey>(_inner, _comparer)
            {
                key
            };
            
            Interlocked.CompareExchange(ref _inner, copy, _inner);
        }
    }
}