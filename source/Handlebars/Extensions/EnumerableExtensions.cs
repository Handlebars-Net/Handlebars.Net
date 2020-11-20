using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.Collections;

namespace HandlebarsDotNet
{
    internal static class EnumerableExtensions
    {
        public static bool Any(this IEnumerable builder)
        {
            var enumerator = builder.GetEnumerator();
            return enumerator.MoveNext();
        }
        
        public static bool IsOneOf<TSource, TExpected>(this IEnumerable<TSource> source)
            where TExpected : TSource
        {
            using var enumerator = source.GetEnumerator();
            enumerator.MoveNext();
            return enumerator.Current is TExpected && !enumerator.MoveNext();
        }
        
        public static bool IsMultiple<T>(this IEnumerable<T> source)
        {
            using var enumerator = source.GetEnumerator();
            var hasNext = enumerator.MoveNext();
            hasNext = hasNext && enumerator.MoveNext();
            return hasNext;
        }

        public static IEnumerable<T> ApplyOn<T, TV>(this IEnumerable<T> source, Action<TV> mutator)
            where T: class
            where TV : T
        {
            foreach (var item in source)
            {
                if(item is TV typed) mutator(typed);
                yield return item;
            }   
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddOrUpdate<TK, TV, TO>(this IDictionary<TK, TV> to, TK at, Func<TO, TV> add, Action<TO, TV> update, TO context)
        {
            if (to.TryGetValue(at, out var value))
            {
                update(context, value);
                return;
            }

            to.Add(at, add(context));
        }

        public static IIndexed<TKey, TValue> ToIndexed<T, TKey, TValue, TComparer>(
            this IEnumerable<T> enumerable,
            Func<T, TKey> keySelector,
            Func<T, TValue> valueSelector,
            TComparer comparer
        ) where TComparer : IEqualityComparer<TKey>
        {
            var dictionary = new DictionarySlim<TKey, TValue, TComparer>(comparer);
            foreach (var item in enumerable)
            {
                dictionary.AddOrReplace(keySelector(item), valueSelector(item));
            }

            return dictionary;
        }

        public static TValue Optional<TKey, TValue>(this IReadOnlyIndexed<TKey, TValue> indexed, in TKey key)
        {
            indexed.TryGetValue(key, out var value);
            return value;
        }

        public static SequenceOfOneClass<T> SequenceOfOne<T>(this T value)
        {
            return new SequenceOfOneClass<T>(value);
        }
        
        internal struct SequenceOfOneClass<T> : IEnumerable<T>, IEnumerator<T>
        {
            private readonly T _value;
            private bool _enumerated;

            public SequenceOfOneClass(T value)
            {
                _value = value;
                _enumerated = false;
                Current = default;
            }

            public SequenceOfOneClass<T> GetEnumerator() => this;
            
            IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            
            public bool MoveNext()
            {
                if (_enumerated)
                {
                    Current = default;
                    return false;
                }
                _enumerated = true;
                Current = _value;
                return true;
            }

            public void Reset() => _enumerated = false;

            public T Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                // nothing to do here
            }
        }

        public static TList AddMany<T, TList>(this TList list, IEnumerable<T> items)
            where TList: IAppendOnlyList<T>
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
            
            return list;
        }
    }
}
