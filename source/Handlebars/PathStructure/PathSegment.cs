using System;
using HandlebarsDotNet.StringUtils;

namespace HandlebarsDotNet.PathStructure
{
    /// <summary>
    /// Represents parts of single <see cref="PathInfo"/> separated with '/'.
    /// </summary>
    public readonly struct PathSegment : IEquatable<PathSegment>, IEquatable<string>
    {
        private static readonly Substring ThisSubstring = "this";
        
        private readonly int _hashCode;
        
        internal readonly bool IsParent;
        internal readonly bool IsThis;
        
        internal PathSegment(Substring segment, ChainSegment[] chain) : this()
        {
            IsNotEmpty = segment.Length != 0;
            IsParent = IsNotEmpty && segment == "..";
            IsThis = IsNotEmpty && !IsParent && (segment == "." || Substring.EqualsIgnoreCase(segment, ThisSubstring));
            PathChain = chain;

            _hashCode = GetHashCodeImpl();
        }
        
        /// <inheritdoc cref="ChainSegment"/>
        public readonly ChainSegment[] PathChain;

        public readonly bool IsNotEmpty;

        public bool Equals(PathSegment other)
        {
            bool basicEquals = IsNotEmpty == other.IsNotEmpty 
                               && other.PathChain.Length == PathChain.Length
                               && IsThis == other.IsThis 
                               && IsParent == other.IsParent;

            if (!basicEquals) return false;
            
            for (var index = 0; index < PathChain.Length; index++)
            {
                if (!PathChain[index].Equals(other.PathChain[index])) return false;
            }

            return true;
        }

        public bool Equals(string other)
        {
            return string.Equals(other, ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return obj is PathSegment other && Equals(other);
        }

        public override int GetHashCode() => _hashCode;

        public override string ToString()
        {
            return string.Join<ChainSegment>(".", PathChain);
        }

        private int GetHashCodeImpl()
        {
            unchecked
            {
                var hashCode = IsNotEmpty.GetHashCode();
                hashCode = (hashCode * 397) ^ IsThis.GetHashCode();
                hashCode = (hashCode * 397) ^ IsParent.GetHashCode();

                for (var index = 0; index < PathChain.Length; index++)
                {
                    hashCode = (hashCode * 397) ^ PathChain[index].GetHashCode();
                }
                
                return hashCode;
            }
        }
    }
}