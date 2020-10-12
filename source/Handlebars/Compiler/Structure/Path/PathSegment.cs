using System;

namespace HandlebarsDotNet.Compiler.Structure.Path
{
    internal delegate object ProcessPathChain(BindingContext context, HashParameterDictionary hashParameters, ref PathInfo pathInfo, ref PathSegment segment, object instance);
    
    /// <summary>
    /// Represents parts of single <see cref="PathInfo"/> separated with '/'.
    /// </summary>
    public struct PathSegment : IEquatable<PathSegment>
    {
        private readonly string _segment;
        
        internal readonly ProcessPathChain ProcessPathChain;
        internal readonly bool IsJumpUp;
        
        internal PathSegment(string segment, ChainSegment[] chain, bool isJumpUp, ProcessPathChain processPathChain)
        {
            _segment = segment;
            IsJumpUp = isJumpUp;
            PathChain = chain;
            ProcessPathChain = processPathChain;
        }

        /// <inheritdoc cref="ChainSegment"/>
        public readonly ChainSegment[] PathChain;

        /// <summary>
        /// Returns string representation of current <see cref="PathSegment"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => _segment;

        /// <inheritdoc />
        public bool Equals(PathSegment other) => _segment == other._segment;

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is PathSegment other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => _segment != null ? _segment.GetHashCode() : 0;

        /// <inheritdoc cref="Equals(HandlebarsDotNet.Compiler.Structure.Path.PathSegment)"/>
        public static bool operator ==(PathSegment a, PathSegment b) => a.Equals(b);

        /// <inheritdoc cref="Equals(HandlebarsDotNet.Compiler.Structure.Path.PathSegment)"/>
        public static bool operator !=(PathSegment a, PathSegment b) => !a.Equals(b);
    }
}