using System;
using System.Collections.Generic;

namespace HandlebarsDotNet.PathStructure
{
    public sealed partial class PathInfo
    {
        internal readonly struct TrimmedPathEqualityComparer : IEqualityComparer<PathInfo>
        {
            private readonly bool _countParts;
            private readonly bool _ignoreCase;
            private readonly StringComparison _stringComparison;

            public TrimmedPathEqualityComparer(bool countParts = true, bool ignoreCase = true)
            {
                _ignoreCase = ignoreCase;
                _countParts = countParts;
                _stringComparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            }
            
            public bool Equals(PathInfo x, PathInfo y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                
                return (!_countParts || x.Segments.Length == y.Segments.Length) 
                       && string.Equals(x.TrimmedPath, y.TrimmedPath, _stringComparison);
            }

            public int GetHashCode(PathInfo obj)
            {
                return _ignoreCase ? obj._trimmedInvariantHashCode : obj._trimmedHashCode;
            }
        }
    }
}