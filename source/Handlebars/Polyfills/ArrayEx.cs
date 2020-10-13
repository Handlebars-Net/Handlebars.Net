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
            return EmptyArray<T>.Value;
#else
            return Array.Empty<T>();
#endif
        }
        
#if !netstandard
        private static class EmptyArray<T>
        {
            public static readonly T[] Value = new T[0];
        }
#endif
    }
}