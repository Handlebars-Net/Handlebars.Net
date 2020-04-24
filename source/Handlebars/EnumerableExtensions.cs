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
    }
}
