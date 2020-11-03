using System;
using System.Collections.Generic;

namespace HandlebarsDotNet.EqualityComparers
{
    internal readonly struct TypeEqualityComparer : IEqualityComparer<Type>
    {
        public bool Equals(Type x, Type y) => ReferenceEquals(x, y) || x == y;

        public int GetHashCode(Type obj) => obj.GetHashCode();
    }
}