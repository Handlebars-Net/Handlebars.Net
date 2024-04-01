using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.Collections
{
    /// <summary>
    /// Append-only data structure of a fixed size that provides dictionary-like lookup capabilities.
    /// <para>Performance of <see cref="AddOrReplace"/>, <see cref="ContainsKey(in TKey)"/> and <see cref="TryGetValue(in TKey, out TValue)"/>
    /// starts to degrade as number of items comes closer to <see cref="Capacity"/>.</para>
    /// <para><see cref="TryGetValue(in EntryIndex(TKey), out TValue)"/> and <see cref="ContainsKey(in EntryIndex(TKey)"/> always performs at constant time.</para>
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public class FixedSizeDictionary<TKey, TValue, TComparer> :
        IIndexed<TKey, TValue>,
        IReadOnlyDictionary<TKey, TValue> 
        where TKey : notnull
        where TValue : notnull
        where TComparer : notnull, IEqualityComparer<TKey>
    {
        private const int MaximumSize = 1024;
        
        private readonly int _bucketMask;
        private readonly int _bucketSize;
        
        private readonly Entry[] _entries;
        private readonly EntryIndex<TKey>[] _indexes; // required for fast cleanup and copy

        private readonly TComparer _comparer;

        private byte _version;
        private int _count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bucketsCount">Actual number of buckets would be the closest power of 2 and multiplied by itself. Maximum size is <c>1024</c>.</param>
        /// <param name="bucketSize">Actual bucket size would be closest prime number. Maximum size is <c>199</c></param>
        /// <param name="comparer"></param>
        public FixedSizeDictionary(int bucketsCount, int bucketSize, TComparer comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            if (bucketsCount > MaximumSize) throw new ArgumentException($" cannot be greater then {MaximumSize}", nameof(bucketsCount));
            if (bucketSize > HashHelper.Primes[HashHelper.Primes.Length - 1]) throw new ArgumentException($" cannot be greater then {HashHelper.Primes[HashHelper.Primes.Length - 1]}", nameof(bucketSize));

            // size is always ^2.
            bucketsCount = HashHelper.AlignBy2(bucketsCount);
            _comparer = comparer;
            _bucketMask = bucketsCount - 1;
            _bucketSize = HashHelper.FindClosestPrime(bucketSize);
            _version = 1;

            _entries = new Entry[bucketsCount * bucketSize];
            _indexes = new EntryIndex<TKey>[bucketsCount * bucketSize];
        }

        public int Count => _count;

        /// <summary>
        /// Amount of items can be added to the dictionary
        /// </summary>
        public int Capacity => _entries.Length;

        /// <summary>
        /// Calculates current index for the given key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool TryGetIndex(TKey key, out EntryIndex<TKey> index)
        {
            if (_count == 0)
            {
                index = default;
                return false;
            }
            
            var hash = _comparer.GetHashCode(key);
            var bucketIndex = hash & _bucketMask;

            var inBucketEntryIndex = hash % _bucketSize;
            var entryIndex = bucketIndex * _bucketSize + Math.Abs(inBucketEntryIndex);
            
            var entry = _entries[entryIndex];
            if (entry.Version != _version || hash == entry.Hash && _comparer.Equals(key, entry.Key))
            {
                index = new EntryIndex<TKey>(entryIndex, _version);
                return true;
            }

            // handling possible collisions
            while (entry.Next != -1)
            {
                entry = _entries[entry.Next];
                if (!entry.IsNotDefault) break;
                if (entry.Version == _version && (hash != entry.Hash || !_comparer.Equals(key, entry.Key))) continue;

                index = new EntryIndex<TKey>(entry.Index, _version);
                return true;
            }

            index = new EntryIndex<TKey>();
            return false;
        }

        /// <summary>
        /// Checks key existence at guarantied O(1) ignoring actual key comparison
        /// </summary>
        public bool ContainsKey(in EntryIndex<TKey> keyIndex)
        {
            // No need to extract actual value. EntryIndex should be used only as part of it's issuer
            // and as collection is append only it's guarantied to have the value at particular index
            return keyIndex.Version == _version;
        }

        /// <summary>
        /// Checks key existence at best O(1) and worst O(m) where 'm' is number of collisions 
        /// </summary>
        public bool ContainsKey(in TKey key)
        {
            if (_count == 0) return false;

            var hash = _comparer.GetHashCode(key);
            var bucketIndex = hash & _bucketMask;

            var inBucketEntryIndex = hash % _bucketSize;
            var entryIndex = bucketIndex * _bucketSize + Math.Abs(inBucketEntryIndex);

            var entry = _entries[entryIndex];
            if (!entry.IsNotDefault || entry.Version != _version) return false;
            if (hash == entry.Hash && _comparer.Equals(key, entry.Key))
            {
                return true;
            }

            // handling possible collisions
            while (entry.Next != -1)
            {
                entry = _entries[entry.Next];
                if (!entry.IsNotDefault || entry.Version != _version) return false;
                if (hash == entry.Hash && _comparer.Equals(key, entry.Key))
                {
                    return true;
                }
            }

            return false;
        }
        
        bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key) => ContainsKey(key);

        /// <summary>
        /// Performs lookup at guarantied O(1) ignoring actual key comparison
        /// </summary>
        public bool TryGetValue(in EntryIndex<TKey> keyIndex, out TValue value)
        {
            if (_count == 0 || keyIndex.Version != _version)
            {
                value = default;
                return false;
            }

            var entry = _entries[keyIndex.Index];
            if (entry.IsNotDefault && entry.Version == _version)
            {
                value = entry.Value;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Performs lookup at best O(1) and worst O(m) where 'm' is number of collisions
        /// </summary>
        public bool TryGetValue(in TKey key, out TValue value)
        {
            if (_count == 0)
            {
                value = default;
                return false;
            }
            
            var hash = _comparer.GetHashCode(key);
            var bucketIndex = hash & _bucketMask;
            
            var inBucketEntryIndex = hash % _bucketSize;
            var entryIndex = bucketIndex * _bucketSize + Math.Abs(inBucketEntryIndex);

            var entry = _entries[entryIndex];
            if (!entry.IsNotDefault || entry.Version != _version)
            {
                value = default;
                return false;
            }

            if (hash == entry.Hash && _comparer.Equals(key, entry.Key))
            {
                value = entry.Value;
                return true;
            }

            // handling possible collisions
            while (entry.Next != -1)
            {
                entry = _entries[entry.Next];
                if (!entry.IsNotDefault || entry.Version != _version)
                {
                    value = default;
                    return false;
                }

                if (hash == entry.Hash && _comparer.Equals(key, entry.Key))
                {
                    value = entry.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public TValue this[in TKey key]
        {
            get => TryGetValue(key, out var value) ? value : default;
            set => AddOrReplace(key, value, out _);
        }

        void IIndexed<TKey, TValue>.AddOrReplace(in TKey key, in TValue value)
        {
            AddOrReplace(key, value, out _);
        }
        
        /// <summary>
        /// Adds or replaces an item at best O(1) and worst O(m) where 'm' is number of collisions
        /// </summary>
        /// <exception cref="InvalidOperationException">Item cannot be added due to capacity constraint.</exception>
        public void AddOrReplace(in TKey key, in TValue value, out EntryIndex<TKey> index)
        {
            var hash = _comparer.GetHashCode(key);
            var bucketIndex = hash & _bucketMask;

            var inBucketEntryIndex = hash % _bucketSize;
            var entryIndex = bucketIndex * _bucketSize + Math.Abs(inBucketEntryIndex);
            
            var entry = _entries[entryIndex];
            if (!entry.IsNotDefault || entry.Version != _version)
            {
                _entries[entryIndex] = new Entry(hash, entryIndex, key, value, _version);
                index = new EntryIndex<TKey>(entryIndex, _version);
                _indexes[_count++] = index;
                return;
            }

            if (hash == entry.Hash && _comparer.Equals(key, entry.Key))
            {
                index = new EntryIndex<TKey>(entryIndex, _version);
                _entries[entryIndex].Value = value;
                return;
            }

            // handling possible collisions
            while (entry.Next != -1)
            {
                entry = _entries[entry.Next];
                if (entry.Version != _version)
                {
                    _entries[entry.Index] = new Entry(hash, entry.Index, key, value, _version);
                    index = new EntryIndex<TKey>(entry.Index, _version);
                    _indexes[_count++] = index;
                    return;
                }

                if (hash == entry.Hash && _comparer.Equals(key, entry.Key))
                {
                    index = new EntryIndex<TKey>(entry.Index, _version);
                    _entries[entry.Index].Value = value;
                    return;
                }
            }

            // found no entry to replace in existing collisions
            // trying to find the first available spot in an array
            
            ref var entryReference = ref _entries[entry.Index];
            entryIndex = entryReference.Index + 1;
            var downstreamEntryIndex = entryIndex - 1;

            for (; entryIndex < _entries.Length; entryIndex++)
            {
                entry = _entries[entryIndex];
                if (entry.IsNotDefault && entry.Version == _version) continue;

                entryReference.Next = entryIndex;
                _entries[entryIndex] = new Entry(hash, entryIndex, key, value, _version);
                index = new EntryIndex<TKey>(entryIndex, _version);
                _indexes[_count++] = index;
                return;
            }

            // we've searched all entries in -> direction, now visiting in  <- direction
            entryIndex = downstreamEntryIndex - 1;
            // handling special case when `downstreamEntryIndex` can result into value >= _entries.Length
            if (entryIndex >= _entries.Length) entryIndex = _entries.Length - 1;
            for (; entryIndex >= 0; entryIndex--)
            {
                entry = _entries[entryIndex];
                if (entry.IsNotDefault && entry.Version == _version) continue;

                entryReference.Next = entryIndex;
                _entries[entryIndex] = new Entry(hash, entryIndex, key, value, _version);
                index = new EntryIndex<TKey>(entryIndex, _version);
                _indexes[_count++] = index;
                return;
            }

            throw new InvalidOperationException("Item cannot be added due to capacity constraint.");
        }
        
        /// <summary>
        /// Gets or replaces item at a given index at O(1)
        /// </summary>
        /// <param name="entryIndex"></param>
        public TValue this[in EntryIndex<TKey> entryIndex]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (entryIndex.Version != _version) return default;
                return _entries[entryIndex.Index].Value;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (entryIndex.Version != _version) return;
                _entries[entryIndex.Index].Value = value;
            }
        }

        /// <summary>
        /// Copies items from one dictionary to another at O(n)
        /// </summary>
        /// <param name="destination"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(FixedSizeDictionary<TKey, TValue, TComparer> destination)
        {
            if (Capacity != destination.Capacity)
                Throw.CapacityShouldBeEqual(nameof(destination));
            
            if(_count == 0) return;
            
            for (var index = 0; index < _indexes.Length; index++)
            {
                var idx = _indexes[index];
                if (idx.Version != _version || !idx.IsNotEmpty)
                {
                    destination._indexes[index] = new EntryIndex<TKey>(idx.Index, destination._version);
                    break;
                }
                
                var entry = _entries[idx.Index];
                if(!entry.IsNotDefault || entry.Version != _version) continue;

                destination._indexes[index] = new EntryIndex<TKey>(idx.Index, destination._version);
                destination._entries[idx.Index] = new Entry(entry, destination._version);
            }
            
            destination._count = _count;
        }

        public void AdjustIndexes(EntryIndex<TKey>[] source, FixedSizeDictionary<TKey, TValue, TComparer> destination, EntryIndex<TKey>[] target)
        {
            if(source.Length != target.Length) Throw.CapacityShouldBeEqual(nameof(target));
            
            for (var i = 0; i < source.Length; i++)
            {
                target[i] = new EntryIndex<TKey>(source[i].Index, destination._version);
            }
        }

        /// <summary>
        /// Performs fast cleanup without cleaning internal storage (does not make objects available for GC)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            _count = 0;
            
            unchecked { ++_version; }
            
            if (_version == 0) _version = 1;
        }

        /// <summary>
        /// Performs full cleanup
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            for (var index = 0; index < _indexes.Length; index++)
            {
                var idx = _indexes[index];
                if(idx.Version != _version || !idx.IsNotEmpty) break;

                _entries[idx.Index] = new Entry();
                _indexes[index] = new EntryIndex<TKey>();
            }

            _count = 0;
            
            unchecked { ++_version; }

            if (_version == 0) _version = 1;
        }
        
        /// <summary>
        /// Performs full cleanup once in 3 versions
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OptionalClear()
        {
            if (_version % 10 == 0)
            {
                Clear();
                return;
            }
            
            Reset();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (var index = 0; index < _indexes.Length; index++)
            {
                var idx = _indexes[index];
                if(idx.Version != _version || !idx.IsNotEmpty) break;
                
                var entry = _entries[idx.Index];
                
                yield return new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value) => TryGetValue(key, out value);

        TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                if (!TryGetIndex(key, out var index)) throw new KeyNotFoundException();
                return this[index];
            }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get
            {
                for (var index = 0; index < _indexes.Length; index++)
                {
                    var idx = _indexes[index];
                    if(idx.Version != _version || !idx.IsNotEmpty) break;

                    yield return _entries[idx.Index].Key;
                }
            }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get
            {
                for (var index = 0; index < _indexes.Length; index++)
                {
                    var idx = _indexes[index];
                    if(idx.Version != _version || !idx.IsNotEmpty) break;

                    yield return _entries[idx.Index].Value;
                }
            }
        }

        private struct Entry
        {
            public readonly int Index;
            public readonly int Hash;
            public readonly TKey Key;
            public readonly bool IsNotDefault;
            public readonly byte Version;
            public int Next;
            public TValue Value;
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Entry(in int hash, in int index, in TKey key, in TValue value, in byte version)
            {
                Index = index;
                Hash = hash;
                Key = key;
                Value = value;
                Version = version;
                IsNotDefault = true;
                Next = -1;
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Entry(Entry entry, byte version)
            {
                Index = entry.Index;
                Hash = entry.Hash;
                Key = entry.Key;
                Value = entry.Value;
                Version = version;
                Next = entry.Next;
                IsNotDefault = true;
            }

            public override string ToString() => $"{Key}: {Value}";
        }
        
        private static class Throw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void CapacityShouldBeEqual(string paramName) => throw new ArgumentException(" capacity should be equal to source dictionary", paramName);
        }
    }

    [DebuggerDisplay("{Index}")]
    public readonly struct EntryIndex<TKey> : IEquatable<EntryIndex<TKey>>
    {
        public readonly int Index;
        public readonly byte Version;
        public readonly bool IsNotEmpty;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal EntryIndex(in int index, in byte version)
        {
            Version = version;
            Index = index;
            IsNotEmpty = true;
        }
        
        public bool Equals(EntryIndex<TKey> other) 
            => IsNotEmpty == other.IsNotEmpty && Version == other.Version && Index == other.Index;

        public override bool Equals(object obj) 
            => obj is EntryIndex<TKey> other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Index;
                hashCode = (hashCode * 397) ^ Version;
                hashCode = (hashCode * 397) ^ IsNotEmpty.GetHashCode();
                return hashCode;
            }
        }
    }
}