using System;
using System.Collections.Generic;

namespace HandlebarsDotNet
{
    internal static class EnumerableExtensions
    {
        public static bool IsOneOf<TSource, TExpected>(this IEnumerable<TSource> source)
            where TExpected : TSource
        {
            using(var enumerator = source.GetEnumerator())
            {
                enumerator.MoveNext();
                return enumerator.Current is TExpected && !enumerator.MoveNext();
            }
        }
        
        public static bool IsMultiple<T>(this IEnumerable<T> source)
        {
            using(var enumerator = source.GetEnumerator())
            {
                var hasNext = enumerator.MoveNext();
                hasNext = hasNext && enumerator.MoveNext();
                return hasNext;
            }
        }

        public static IEnumerable<T> Apply<T>(this IEnumerable<T> source, Action<T> mutator)
            where T: class
        {
            foreach (var item in source)
            {
                mutator(item);
                yield return item;
            }   
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
    }
}
