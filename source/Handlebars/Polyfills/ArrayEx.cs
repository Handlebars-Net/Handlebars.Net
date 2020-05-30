using System;

namespace HandlebarsDotNet.Polyfills
{
    internal static class ArrayEx
    {
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