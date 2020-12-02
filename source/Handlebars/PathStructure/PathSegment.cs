using HandlebarsDotNet.StringUtils;

namespace HandlebarsDotNet.PathStructure
{
    /// <summary>
    /// Represents parts of single <see cref="PathInfo"/> separated with '/'.
    /// </summary>
    public readonly struct PathSegment
    {
        internal readonly bool IsContextChange;
        internal readonly bool IsThis;
        
        internal PathSegment(Substring segment, ChainSegment[] chain)
        {
            IsNotEmpty = segment.Length != 0;
            IsContextChange = IsNotEmpty && segment == "..";
            IsThis = IsNotEmpty && !IsContextChange && segment == ".";
            PathChain = chain;
        }
        
        /// <inheritdoc cref="ChainSegment"/>
        public readonly ChainSegment[] PathChain;

        public readonly bool IsNotEmpty;
    }
}