using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet
{
    public readonly struct PathInfoLight : 
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

        internal static IEqualityComparer<PathInfoLight> PlainPathComparer { get; } = new EqualityComparer(false);

        internal static IEqualityComparer<PathInfoLight> PlainPathWithPartsCountComparer { get; } = new EqualityComparer();
        
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
        
        public static implicit operator PathInfoLight(PathInfo pathInfo) => new PathInfoLight(pathInfo);
        
        public static implicit operator PathInfo(PathInfoLight pathInfo) => pathInfo.PathInfo;
        
        private sealed class EqualityComparer : IEqualityComparer<PathInfoLight>
        {
            private readonly PathInfo.TrimmedPathEqualityComparer _comparer;

            public EqualityComparer(bool countParts = true)
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