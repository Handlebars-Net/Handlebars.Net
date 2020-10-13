using System;
using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.Polyfills
{
    internal static class ArrayEx
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Empty<T>()
        {
#if !netstandard
            return new T[0];
#else
            return Array.Empty<T>();
#endif
        }
    }
}