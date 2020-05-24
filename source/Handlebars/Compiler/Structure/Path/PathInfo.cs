using System;

namespace HandlebarsDotNet.Compiler.Structure.Path
{
    internal delegate object ProcessSegment(ref PathInfo pathInfo, ref BindingContext context, object instance, HashParameterDictionary hashParameters);
    
    internal struct PathInfo : IEquatable<PathInfo>
    {
        public PathInfo(bool hasValue, string path, PathSegment[] segments, ProcessSegment processSegment)
        {
            HasValue = hasValue;
            Path = path;
            IsVariable = path.StartsWith("@");
            IsInversion = path.StartsWith("^");
            IsHelper = path.StartsWith("#");
            Segments = segments;
            ProcessSegment = processSegment;
        }

        public readonly bool IsHelper;
        public readonly bool IsInversion;
        public readonly bool HasValue;
        public readonly string Path;
        public readonly bool IsVariable;
        public readonly PathSegment[] Segments;

        public readonly ProcessSegment ProcessSegment;

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

        public override string ToString()
        {
            return Path;
        }

        public static implicit operator string(PathInfo pathInfo)
        {
            return pathInfo.Path;
        }
    }
}