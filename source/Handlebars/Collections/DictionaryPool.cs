using System;
using System.Collections.Generic;

namespace HandlebarsDotNet.Collections
{
    internal class DictionaryPool<TKey, TValue> : InternalObjectPool<Dictionary<TKey, TValue>>
    {
        private static readonly Lazy<DictionaryPool<TKey, TValue>> Self = 
            new Lazy<DictionaryPool<TKey, TValue>>(() => new DictionaryPool<TKey, TValue>(EqualityComparer<TKey>.Default));

        public static DictionaryPool<TKey, TValue> Shared => Self.Value;
        
        public DictionaryPool(IEqualityComparer<TKey> comparer) : base(new Policy(comparer))
        {
        }

        public DictionaryPool(IInternalObjectPoolPolicy<Dictionary<TKey, TValue>> policy) : base(policy)
        {
        }
        
        private class Policy : IInternalObjectPoolPolicy<Dictionary<TKey, TValue>>
        {
            private readonly IEqualityComparer<TKey> _comparer;

            public Policy(IEqualityComparer<TKey> comparer) => _comparer = comparer;

            public Dictionary<TKey, TValue> Create() => new Dictionary<TKey, TValue>(_comparer);

            public bool Return(Dictionary<TKey, TValue> item)
            {
                item.Clear();
                return true;
            }
        }
    }
}