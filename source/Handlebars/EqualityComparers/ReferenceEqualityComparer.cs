using System.Collections.Generic;

namespace HandlebarsDotNet.EqualityComparers
{
    public readonly struct ReferenceEqualityComparer<T> : IEqualityComparer<T>
        where T: class
    {
        public bool Equals(T x, T y) => ReferenceEquals(x, y);

        public int GetHashCode(T obj) => obj.GetHashCode();
    }
}