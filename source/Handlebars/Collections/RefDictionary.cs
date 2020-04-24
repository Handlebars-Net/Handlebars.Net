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
    /// ! Collection does not guaranty successful write in concurrent scenarios: writes may be lost but it guaranties to return a value.   
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal class RefDictionary<TKey, TValue> : IRefDictionary<TKey, TValue> where TValue : struct
    {
        private readonly IEqualityComparer<TKey> _comparer;
        private Container _container;

        public RefDictionary(int capacity = 16, IEqualityComparer<TKey> comparer = null)
        {
            var initialCapacity = HashHelpers.GetPrime(capacity);
            _container = new Container(new int[initialCapacity], new Entry[initialCapacity]);
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        public int Count => _container.Count;

        public bool ContainsKey(TKey key)
        {
            var container = _container;
            var entries = container.Entries;
            var entryIndex = GetEntryIndex(key, container);

            while (entryIndex != -1)
            {
                if (_comparer.Equals(entries[entryIndex].Key, key)) return true;

                entryIndex = entries[entryIndex].Next;
            }

            return false;
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
                var container = _container;
                var entries = container.Entries;

                var entryIndex = GetEntryIndex(key, container);
                while (entryIndex != -1)
                {
                    if (_comparer.Equals(entries[entryIndex].Key, key))
                    {
                        return ref entries[entryIndex].Value;
                    }

                    entryIndex = entries[entryIndex].Next;
                }

                if (Count == entries.Length && !TryResize(container, out container))
                {
                    var entry = new Entry(key);
                    var holder = new CollisionHolder(ref entry);
                    return ref holder.Entry.Value;
                }

                entries = container.Entries;
                var buckets = container.Buckets;

                entryIndex = container.Count++;
                entries[entryIndex].Key = key;
                var bucket = GetBucketIndex(key, container);
                entries[entryIndex].Next = buckets[bucket] - 1;
                buckets[bucket] = entryIndex + 1;
                return ref entries[entryIndex].Value;
            }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get
            {
                var container = _container;
                var entries = container.Entries;
                for (var index = 0; index < container.Count; index++)
                {
                    yield return entries[index].Key;
                }
            }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get
            {
                var container = _container;
                var entries = container.Entries;
                for (var index = 0; index < container.Count; index++)
                {
                    yield return entries[index].Value;
                }
            }
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            var container = _container;
            var entries = container.Entries;
            for (var index = 0; index < Count; index++)
            {
                var entry = entries[index];
                yield return new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetBucketIndex(TKey key, Container container) =>
            (_comparer.GetHashCode(key) & 0x7FFFFFFF) % container.Buckets.Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetEntryIndex(TKey key, Container container) =>
            container.Buckets[GetBucketIndex(key, container)] - 1;

        private bool TryResize(Container container, out Container newContainer)
        {
            var entries = container.Entries;

            var count = container.Count;
            var newSize = HashHelpers.ExpandPrime(count);

            var newEntries = new Entry[newSize];
            Array.Copy(entries, 0, newEntries, 0, count);

            var newBuckets = new int[newSize];

            newContainer = new Container(newBuckets, newEntries, count);
            for (var index = 0; index < count;)
            {
                var bucketIndex = GetBucketIndex(newEntries[index].Key, newContainer);
                newEntries[index].Next = newBuckets[bucketIndex] - 1;
                newBuckets[bucketIndex] = ++index;
            }

            return ReferenceEquals(Interlocked.CompareExchange(ref _container, newContainer, container), container);
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

        private class CollisionHolder
        {
            public Entry Entry;

            public CollisionHolder(ref Entry entry)
            {
                Entry = entry;
            }
        }

        private class Container
        {
            public readonly int[] Buckets;
            public readonly Entry[] Entries;
            public int Count;

            public Container(int[] buckets, Entry[] entries, int count = 0)
            {
                Buckets = buckets;
                Entries = entries;
                Count = count;
            }
        }
    }
}