// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.Collections
{
    [DebuggerDisplay("Count = {Count}")]
    internal class DictionarySlim<TKey, TValue, TComparer> :
        IIndexed<TKey, TValue>
        where TComparer: IEqualityComparer<TKey>
    {
        private readonly TComparer _comparer;

        // We want to initialize without allocating arrays. We also want to avoid null checks.
        // Array.Empty would give divide by zero in modulo operation. So we use static one element arrays.
        // The first add will cause a resize replacing these with real arrays of three elements.
        // Arrays are wrapped in a class to avoid being duplicated for each <TKey, TValue>
        private static readonly Entry[] InitialEntries = new Entry[1];
        private int _count;
        // 0-based index into _entries of head of free chain: -1 means empty
        private int _freeList = -1;
        // 1-based index into _entries; 0 means empty
        private int[] _buckets;
        private Entry[] _entries;


        [DebuggerDisplay("({Key}, {Value})->{Next}")]
        private struct Entry
        {
            public TKey Key;
            public TValue Value;
            // 0-based index of next entry in chain: -1 means end of chain
            // also encodes whether this entry _itself_ is part of the free list by changing sign and subtracting 3,
            // so -2 means end of free list, -3 means index 0 but on free list, -4 means index 1 but on free list, etc.
            public int Next;
        }

        /// <summary>
        /// Construct with default capacity.
        /// </summary>
        public DictionarySlim(TComparer comparer)
        {
            _comparer = comparer;
            _buckets = HashHelper.SizeOneIntArray;
            _entries = InitialEntries;
        }
        
        public DictionarySlim(int capacity, TComparer comparer)
        {
            if (capacity < 0)
                throw new ArgumentException(nameof(capacity));
           
            if (capacity < 2)
                capacity = 2; // 1 would indicate the dummy array
            
            _comparer = comparer;
            capacity = HashHelper.PowerOf2(capacity);
            _buckets = new int[capacity];
            _entries = new Entry[capacity];
        }

        public DictionarySlim(IReadOnlyIndexed<TKey, TValue> other, TComparer comparer)
            :this(other.Count, comparer)
        {
            foreach (var pair in other)
            {
                AddOrReplace(pair.Key, pair.Value);
            }
        }
        
        public DictionarySlim(DictionarySlim<TKey, TValue, TComparer> other)
        {
            _comparer = other._comparer;
            _buckets = new int[other._buckets.Length];
            _entries = new Entry[other._entries.Length];

            var enumerator = new Enumerator(other);
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                AddOrReplace(current.Key, current.Value);
            }
        }

        /// <summary>
        /// Count of entries in the dictionary.
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// Clears the dictionary. Note that this invalidates any active enumerators.
        /// </summary>
        public void Clear()
        {
            _count = 0;
            _freeList = -1;
            for (var index = 0; index < _entries.Length; index++)
            {
                _buckets[index] = default;
                _entries[index] = default;
            }
        }

        /// <summary>
        /// Looks for the specified key in the dictionary.
        /// </summary>
        /// <param name="key">Key to look for</param>
        /// <returns>true if the key is present, otherwise false</returns>
        public bool ContainsKey(in TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var entries = _entries;
            var collisionCount = 0;
            for (var i = _buckets[_comparer.GetHashCode(key) & (_buckets.Length-1)] - 1;
                    (uint)i < (uint)entries.Length; i = entries[i].Next)
            {
                if (_comparer.Equals(key, entries[i].Key))
                    return true;
                
                if (collisionCount == entries.Length)
                {
                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    throw new InvalidOperationException("ConcurrentOperationsNotSupported");
                }
                collisionCount++;
            }

            return false;
        }

        /// <summary>
        /// Gets the value if present for the specified key.
        /// </summary>
        /// <param name="key">Key to look for</param>
        /// <param name="value">Value found, otherwise default(TValue)</param>
        /// <returns>true if the key is present, otherwise false</returns>
        public bool TryGetValue(in TKey key, out TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var entries = _entries;
            var collisionCount = 0;
            for (var i = _buckets[_comparer.GetHashCode(key) & (_buckets.Length - 1)] - 1;
                    (uint)i < (uint)entries.Length; i = entries[i].Next)
            {
                if (_comparer.Equals(key, entries[i].Key))
                {
                    value = entries[i].Value;
                    return true;
                }
                if (collisionCount == entries.Length)
                {
                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    throw new InvalidOperationException("ConcurrentOperationsNotSupported");
                }
                collisionCount++;
            }

            value = default;
            return false;
        }

        public TValue this[in TKey key]
        {
            get => TryGetValue(key, out var value) ? value : throw new KeyNotFoundException($"{key}");
            set => AddOrReplace(key, value);
        }

        public void AddOrReplace(in TKey key, in TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var entries = _entries;
            var collisionCount = 0;
            var bucketIndex = _comparer.GetHashCode(key) & (_buckets.Length - 1);
            for (var i = _buckets[bucketIndex] - 1;
                (uint)i < (uint)entries.Length; i = entries[i].Next)
            {
                if (_comparer.Equals(key, entries[i].Key))
                    entries[i].Value = value;
                if (collisionCount == entries.Length)
                {
                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    throw new InvalidOperationException("ConcurrentOperationsNotSupported");
                }
                collisionCount++;
            }

            AddValue(key, value, bucketIndex);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddValue(TKey key, TValue value, int bucketIndex)
        {
            var entries = _entries;
            int entryIndex;
            if (_freeList != -1)
            {
                entryIndex = _freeList;
                _freeList = -3 - entries[_freeList].Next;
            }
            else
            {
                if (_count == entries.Length || entries.Length == 1)
                {
                    entries = Resize();
                    bucketIndex = _comparer.GetHashCode(key) & (_buckets.Length - 1);
                    // entry indexes were not changed by Resize
                }
                entryIndex = _count;
            }

            entries[entryIndex].Key = key;
            entries[entryIndex].Next = _buckets[bucketIndex] - 1;
            _buckets[bucketIndex] = entryIndex + 1;
            _count++;
            entries[entryIndex].Value = value;
        }

        private Entry[] Resize()
        {
            var count = _count;
            var newSize = _entries.Length * 2;
            if ((uint)newSize > (uint)int.MaxValue) // uint cast handles overflow
                throw new InvalidOperationException("capacity overflow");

            var entries = new Entry[newSize];
            Array.Copy(_entries, 0, entries, 0, count);

            var newBuckets = new int[entries.Length];
            while (count-- > 0)
            {
                var bucketIndex = _comparer.GetHashCode(entries[count].Key) & (newBuckets.Length - 1);
                entries[count].Next = newBuckets[bucketIndex] - 1;
                newBuckets[bucketIndex] = count + 1;
            }

            _buckets = newBuckets;
            _entries = entries;

            return entries;
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Enumerator
        /// </summary>
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly DictionarySlim<TKey, TValue, TComparer> _dictionary;
            private int _index;
            private int _count;
            private KeyValuePair<TKey, TValue> _current;

            internal Enumerator(DictionarySlim<TKey, TValue, TComparer> dictionary)
            {
                _dictionary = dictionary;
                _index = 0;
                _count = _dictionary._count;
                _current = default;
            }

            /// <summary>
            /// Move to next
            /// </summary>
            public bool MoveNext()
            {
                if (_count == 0)
                {
                    _current = default;
                    return false;
                }

                _count--;

                while (_dictionary._entries[_index].Next < -1)
                    _index++;

                _current = new KeyValuePair<TKey, TValue>(
                    _dictionary._entries[_index].Key,
                    _dictionary._entries[_index++].Value);
                return true;
            }

            /// <summary>
            /// Get current value
            /// </summary>
            public KeyValuePair<TKey, TValue> Current => _current;

            object IEnumerator.Current => _current;

            void IEnumerator.Reset()
            {
                _index = 0;
                _count = _dictionary._count;
                _current = default;
            }

            /// <summary>
            /// Dispose the enumerator
            /// </summary>
            public void Dispose() { }
        }
    }
}