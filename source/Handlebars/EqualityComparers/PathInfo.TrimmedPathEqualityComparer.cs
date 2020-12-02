using System.Collections.Generic;

namespace HandlebarsDotNet.PathStructure
{
    public sealed partial class PathInfo
    {
        internal readonly struct TrimmedPathEqualityComparer : IEqualityComparer<PathInfo>
        {
            private readonly bool _countParts;

            public TrimmedPathEqualityComparer(bool countParts = true)
            {
                _countParts = countParts;
            }
            
            public bool Equals(PathInfo x, PathInfo y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                
                return (!_countParts || x.PathChain.Length == y.PathChain.Length) && string.Equals(x.TrimmedPath, y.TrimmedPath);
            }

            public int GetHashCode(PathInfo obj)
            {
                return obj._trimmedHashCode;
            }
        }
    }
}