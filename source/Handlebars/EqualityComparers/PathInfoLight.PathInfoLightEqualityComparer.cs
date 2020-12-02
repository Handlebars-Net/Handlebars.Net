using System.Collections.Generic;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet
{
    public readonly partial struct PathInfoLight
    {
        internal sealed class PathInfoLightEqualityComparer : IEqualityComparer<PathInfoLight>
        {
            private readonly PathInfo.TrimmedPathEqualityComparer _comparer;

            public PathInfoLightEqualityComparer(bool countParts = true)
            {
                _comparer = new PathInfo.TrimmedPathEqualityComparer(countParts);
            }
            
            public bool Equals(PathInfoLight x, PathInfoLight y)
            {
                return x._comparerTag == y._comparerTag && _comparer.Equals(x.PathInfo, y.PathInfo);
            }

            public int GetHashCode(PathInfoLight obj)
            {
                return _comparer.GetHashCode(obj.PathInfo);
            }
        }
    }
}