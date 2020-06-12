using System;

namespace HandlebarsDotNet.Compiler.Structure.Path
{
    internal delegate object ProcessSegment(ref PathInfo pathInfo, ref BindingContext context, object instance, HashParameterDictionary hashParameters);
    
    /// <summary>
    /// Represents path expression
    /// </summary>
    public struct PathInfo : IEquatable<PathInfo>
    {
        private readonly string _path;
        
        internal readonly ProcessSegment ProcessSegment;
        internal readonly bool IsValidHelperLiteral;
        internal readonly bool HasValue;
        internal readonly bool IsThis;
        
        internal PathInfo(
            bool hasValue, 
            string path, 
            bool isValidHelperLiteral, 
            PathSegment[] segments,
            ProcessSegment processSegment
        )
        {
            IsValidHelperLiteral = isValidHelperLiteral;
            HasValue = hasValue;
            _path = path;
            IsVariable = path.StartsWith("@");
            Segments = segments;
            ProcessSegment = processSegment;
            IsThis = string.Equals(path, "this", StringComparison.OrdinalIgnoreCase) || path == ".";
        }

        /// <summary>
        /// Indicates whether <see cref="PathInfo"/> is part of <c>@</c> variable
        /// </summary>
        public readonly bool IsVariable;
        
        /// <inheritdoc cref="PathSegment"/>
        public readonly PathSegment[] Segments;

        /// <inheritdoc />
        public bool Equals(PathInfo other)
        {
            return HasValue == other.HasValue 
                   && IsVariable == other.IsVariable 
                   && _path == other._path;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is PathInfo other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => _path.GetHashCode();

        /// <summary>
        /// Returns string representation of current <see cref="PathInfo"/>
        /// </summary>
        public override string ToString() => _path;

        /// <inheritdoc cref="ToString"/>
        public static implicit operator string(PathInfo pathInfo) => pathInfo._path;
    }
}