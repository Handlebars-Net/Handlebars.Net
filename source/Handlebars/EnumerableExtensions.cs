using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Handlebars
{
    internal static class EnumerableExtensions
    {
        public static bool IsOneOf<TSource, TExpected>(this IEnumerable<TSource> source)
            where TExpected : TSource
        {
            var enumerator = source.GetEnumerator();
            enumerator.MoveNext();
            return (enumerator.Current is TExpected) && (enumerator.MoveNext() == false);
        }
    }
}
