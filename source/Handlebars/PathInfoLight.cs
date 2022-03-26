using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet
{
    public readonly partial struct PathInfoLight : 
        IEquatable<PathInfoLight>, 
        IEquatable<PathInfo>
    {
        private readonly int _comparerTag;
        
        public readonly PathInfo PathInfo;

        public PathInfoLight(PathInfo pathInfo)
        {
            PathInfo = pathInfo;
            _comparerTag = 0;
        }
        
        private PathInfoLight(PathInfo pathInfo, int comparerTag)
        {
            PathInfo = pathInfo;
            _comparerTag = comparerTag;
        }

        internal static PathInfoLightEqualityComparer PlainPathComparer { get; } = new PathInfoLightEqualityComparer(countParts: false, ignoreCase: true);

        internal static PathInfoLightEqualityComparer PlainPathWithPartsCountComparer { get; } = new PathInfoLightEqualityComparer(countParts: true, ignoreCase: true);
        
        /// <summary>
        /// Used for special handling of Relaxed Helper Names
        /// </summary>
        [Pure]
        internal PathInfoLight TagComparer()
        {
            return new PathInfoLight(PathInfo, _comparerTag + 1);
        }

        public bool Equals(PathInfoLight other)
        {
            return _comparerTag == other._comparerTag && Equals(PathInfo, other.PathInfo);
        }
        
        public bool Equals(PathInfo other)
        {
            return Equals(PathInfo, other);
        }

        public override bool Equals(object obj)
        {
            return obj is PathInfoLight other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_comparerTag * 397) ^ (PathInfo != null ? PathInfo.GetHashCode() : 0);
            }
        }

        public override string ToString() => PathInfo.ToString();

        public static implicit operator PathInfoLight(PathInfo pathInfo) => new PathInfoLight(pathInfo);
        
        public static implicit operator PathInfoLight(string path) => new PathInfoLight(PathInfoStore.Current?.GetOrAdd(path) ?? PathInfo.Parse(path));
        
        public static implicit operator PathInfo(PathInfoLight pathInfo) => pathInfo.PathInfo;
    }
}