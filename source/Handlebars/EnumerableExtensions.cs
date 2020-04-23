using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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
                return enumerator.MoveNext() && enumerator.MoveNext();
            }
        }
    }
    
    internal static class ObjectExtensions
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T As<T>(this object source) => (T) source;
    }
}
