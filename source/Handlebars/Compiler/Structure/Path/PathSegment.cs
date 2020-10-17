using System;
using System.Collections.Generic;

namespace HandlebarsDotNet.Compiler.Structure.Path
{
    /// <summary>
    /// Represents parts of single <see cref="PathInfo"/> separated with '/'.
    /// </summary>
    public class PathSegment : IEquatable<PathSegment>
    {
        private readonly string _segment;
        
        internal readonly bool IsContextChange;
        internal readonly bool IsThis;
        
        internal PathSegment(string segment, ChainSegment[] chain)
        {
            _segment = segment;
            IsContextChange = string.Equals("..", segment);;
            IsThis = string.Equals(".", segment);
            PathChain = chain;
        }
        
        /// <inheritdoc cref="ChainSegment"/>
        public readonly ChainSegment[] PathChain;

        /// <summary>
        /// Returns string representation of current <see cref="PathSegment"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => _segment;

        /// <inheritdoc />
        public bool Equals(PathSegment other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsContextChange == other.IsContextChange && _segment == other._segment;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PathSegment) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((_segment != null ? _segment.GetHashCode() : 0) * 397) ^ IsContextChange.GetHashCode();
            }
        }

        /// <inheritdoc cref="Equals(HandlebarsDotNet.Compiler.Structure.Path.PathSegment)"/>
        public static bool operator ==(PathSegment a, PathSegment b) => EqualityComparer<PathSegment>.Default.Equals(a, b);

        /// <inheritdoc cref="Equals(HandlebarsDotNet.Compiler.Structure.Path.PathSegment)"/>
        public static bool operator !=(PathSegment a, PathSegment b) => !EqualityComparer<PathSegment>.Default.Equals(a, b);
    }
}