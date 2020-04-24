using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace HandlebarsDotNet.Collections
{
    /// <summary>
    /// Represents <see cref="IDictionary{TKey,TValue}"/>-like collection optimized for storing <see langword="struct"/>s.
    /// The class API is very limited due to performance demands.
    /// ! Collection guaranties successful write  
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal class RefDictionarySafe<TKey, TValue> : IRefDictionary<TKey, TValue>
        where TValue : struct
    {
        private readonly ReaderWriterLockSlim _readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly IEqualityComparer<TKey> _comparer;
        private readonly Container _container;

        public RefDictionarySafe(int capacity = 16, IEqualityComparer<TKey> comparer = null)
        {
            var initialCapacity = HashHelpers.GetPrime(capacity);
            _container = new Container(new int[initialCapacity], new Entry[initialCapacity]);
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        public int Count
        {
            get
            {
                using (_readerWriterLock.UseRead())
                {
                    return _container.Count;
                }
            }
        }

        public bool ContainsKey(TKey key)
        {
            using (_readerWriterLock.UseRead())
            {
                var entries = _container.Entries;
                var entryIndex = GetEntryIndex(key, _container);

                while (entryIndex != -1)
                {
                    if (_comparer.Equals(entries[entryIndex].Key, key)) return true;

                    entryIndex = entries[entryIndex].Next;
                }

                return false;
            }
        }

        bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            if (!ContainsKey(key))
            {
                value = default(TValue);
                return false;
            }

            value = this[key];
            return true;
        }

        TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key] =>
            ContainsKey(key) ? this[key] : default(TValue);

        public ref TValue this[TKey key]
        {
            get
            {
                using (_readerWriterLock.UseRead())
                {
                    var entryIndex = GetEntryIndex(key, _container);
                    while (entryIndex != -1)
                    {
                        if (_comparer.Equals(_container.Entries[entryIndex].Key, key))
                        {
                            return ref _container.Entries[entryIndex].Value;
                        }

                        entryIndex = _container.Entries[entryIndex].Next;
                    }
                }

                using (_readerWriterLock.UseWrite())
                {
                    if (Count == _container.Entries.Length)
                    {
                        Resize();
                    }

                    var entryIndex = _container.Count++;
                    _container.Entries[entryIndex].Key = key;
                    var bucket = GetBucketIndex(key, _container);
                    _container.Entries[entryIndex].Next = _container.Buckets[bucket] - 1;
                    _container.Buckets[bucket] = entryIndex + 1;
                    return ref _container.Entries[entryIndex].Value;
                }
            }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get
            {
                using (_readerWriterLock.UseRead())
                {
                    var entries = _container.Entries;
                    for (var index = 0; index < _container.Count; index++)
                    {
                        yield return entries[index].Key;
                    }
                }
            }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get
            {
                using (_readerWriterLock.UseRead())
                {
                    var entries = _container.Entries;
                    for (var index = 0; index < _container.Count; index++)
                    {
                        yield return entries[index].Value;
                    }
                }
            }
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            using (_readerWriterLock.UseRead())
            {
                var entries = _container.Entries;
                for (var index = 0; index < Count; index++)
                {
                    var entry = entries[index];
                    yield return new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetBucketIndex(TKey key, Container container)
        {
            using (_readerWriterLock.UseRead())
            {
                return (_comparer.GetHashCode(key) & 0x7FFFFFFF) % container.Buckets.Length;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetEntryIndex(TKey key, Container container)
        {
            using (_readerWriterLock.UseRead())
            {
                return container.Buckets[GetBucketIndex(key, container)] - 1;
            }
        }

        private void Resize()
        {
            using (_readerWriterLock.UseWrite())
            {
                var count = _container.Count;
                var newSize = HashHelpers.ExpandPrime(count);

                var newEntries = new Entry[newSize];
                Array.Copy(_container.Entries, 0, newEntries, 0, count);

                var newBuckets = new int[newSize];

                var newContainer = new Container(newBuckets, newEntries, count);
                for (var index = 0; index < count;)
                {
                    var bucketIndex = GetBucketIndex(newEntries[index].Key, newContainer);
                    newEntries[index].Next = newBuckets[bucketIndex] - 1;
                    newBuckets[bucketIndex] = ++index;
                }
                
                _container.Update(newContainer);
            }
        }

        private struct Entry
        {
            public Entry(TKey key) : this()
            {
                Key = key;
            }

            public TKey Key;
            public TValue Value;
            public int Next;
        }
        
        private class Container
        {
            public int[] Buckets { get; private set; }
            public Entry[] Entries { get; private set; }
            public int Count { get; set; }

            public Container(int[] buckets, Entry[] entries, int count = 0)
            {
                Buckets = buckets;
                Entries = entries;
                Count = count;
            }

            public void Update(Container container)
            {
                Buckets = container.Buckets;
                Entries = container.Entries;
                Count = container.Count;
            }
        }
    }
}