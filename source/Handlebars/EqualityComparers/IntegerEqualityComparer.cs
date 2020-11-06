using System.Collections.Generic;

namespace HandlebarsDotNet.EqualityComparers
{
    internal readonly struct IntegerEqualityComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y) => x == y;

        public int GetHashCode(int obj) => obj.GetHashCode();
    }
}