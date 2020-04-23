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
    internal class RefDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
        where TValue : struct
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

        private static class HashHelpers
        {
            // This is the maximum prime smaller than Array.MaxArrayLength
            private const int MaxPrimeArrayLength = 0x7FEFFFFD;

            private const int HashPrime = 101;

            // Table of prime numbers to use as hash table sizes.
            // A typical resize algorithm would pick the smallest prime number in this array
            // that is larger than twice the previous capacity.
            // Suppose our Hashtable currently has capacity x and enough elements are added
            // such that a resize needs to occur. Resizing first computes 2x then finds the
            // first prime in the table greater than 2x, i.e. if primes are ordered
            // p_1, p_2, ..., p_i, ..., it finds p_n such that p_n-1 < 2x < p_n.
            // Doubling is important for preserving the asymptotic complexity of the
            // hashtable operations such as add.  Having a prime guarantees that double
            // hashing does not lead to infinite loops.  IE, your hash function will be
            // h1(key) + i*h2(key), 0 <= i < size.  h2 and the size must be relatively prime.
            // We prefer the low computation costs of higher prime numbers over the increased
            // memory allocation of a fixed prime number i.e. when right sizing a HashSet.
            private static readonly int[] Primes =
            {
                3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
                1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
                17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
                187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
                1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369
            };

            private static bool IsPrime(int candidate)
            {
                if ((candidate & 1) == 0) return candidate == 2;

                var limit = (int) Math.Sqrt(candidate);
                for (var divisor = 3; divisor <= limit; divisor += 2)
                {
                    if ((candidate % divisor) == 0)
                        return false;
                }

                return true;
            }

            public static int GetPrime(int min)
            {
                if (min < 0)
                    throw new ArgumentException();

                for (var i = 0; i < Primes.Length; i++)
                {
                    var prime = Primes[i];
                    if (prime >= min)
                        return prime;
                }

                //outside of our predefined table.
                //compute the hard way.
                for (var i = (min | 1); i < int.MaxValue; i += 2)
                {
                    if (IsPrime(i) && ((i - 1) % HashPrime != 0))
                        return i;
                }

                return min;
            }

            // Returns size of hashtable to grow to.
            public static int ExpandPrime(int oldSize)
            {
                var newSize = 2 * oldSize;

                // Allow the hashtables to grow to maximum possible size (~2G elements) before encountering capacity overflow.
                // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
                if ((uint) newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize)
                {
                    return MaxPrimeArrayLength;
                }

                return GetPrime(newSize);
            }
        }
    }
}