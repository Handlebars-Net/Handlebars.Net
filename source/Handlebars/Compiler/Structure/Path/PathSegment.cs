using System;

namespace HandlebarsDotNet.Compiler.Structure.Path
{
    internal delegate object ProcessPathChain(BindingContext context, HashParameterDictionary hashParameters, ref PathInfo pathInfo, ref PathSegment segment, object instance);
    
    internal struct PathSegment : IEquatable<PathSegment>
    {
        public PathSegment(string segment, ChainSegment[] chain, bool isJumpUp, ProcessPathChain processPathChain)
        {
            Segment = segment;
            IsJumpUp = isJumpUp;
            PathChain = chain;
            ProcessPathChain = processPathChain;
        }

        public readonly string Segment;

        public readonly bool IsJumpUp;

        public readonly ChainSegment[] PathChain;

        public readonly ProcessPathChain ProcessPathChain;

        public override string ToString()
        {
            return Segment;
        }

        public bool Equals(PathSegment other)
        {
            return Segment == other.Segment;
        }

        public override bool Equals(object obj)
        {
            return obj is PathSegment other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Segment != null ? Segment.GetHashCode() : 0;
        }

        public static bool operator ==(PathSegment a, PathSegment b)
        {
            return a.Equals(b);
        }
        
        public static bool operator !=(PathSegment a, PathSegment b)
        {
            return !a.Equals(b);
        }
    }
}