using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HandlebarsDotNet.Compiler.Structure.Path
{
    [DebuggerDisplay("{Segment}")]
    internal struct PathSegment
    {
        public PathSegment(string segment, IEnumerable<ChainSegment> chain, bool isJumpUp)
        {
            Segment = segment;
            IsJumpUp = isJumpUp;
            PathChain = chain.ToArray();
        }
        
        public string Segment { get; }

        public bool IsJumpUp { get; }

        public ChainSegment[] PathChain { get; }
    }
}