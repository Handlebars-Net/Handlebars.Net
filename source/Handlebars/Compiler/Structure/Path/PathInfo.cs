using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HandlebarsDotNet.Compiler.Structure.Path
{
    [DebuggerDisplay("{Path}")]
    internal struct PathInfo
    {
        public PathInfo(bool hasValue, string path, IEnumerable<PathSegment> segments)
        {
            HasValue = hasValue;
            Path = path;
            IsVariable = path.StartsWith("@");
            IsInversion = path.StartsWith("^");
            IsHelper = path.StartsWith("#");
            Segments = segments?.ToArray();
        }

        public bool IsHelper { get; }
        public bool IsInversion { get; }
        public bool HasValue { get; }
        public string Path { get; }
        public bool IsVariable { get; }
        public PathSegment[] Segments { get; }

        public bool Equals(PathInfo other)
        {
            return IsHelper == other.IsHelper && 
                   IsInversion == other.IsInversion && 
                   HasValue == other.HasValue && IsVariable == other.IsVariable && 
                   Path == other.Path;
        }

        public override bool Equals(object obj)
        {
            return obj is PathInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }

        public static implicit operator string(PathInfo pathInfo)
        {
            return pathInfo.Path;
        }
    }
}