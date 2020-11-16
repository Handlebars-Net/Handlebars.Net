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
    }
}
