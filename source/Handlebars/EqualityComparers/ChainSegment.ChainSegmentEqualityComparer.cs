using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.PathStructure
{
    public sealed partial class ChainSegment
    {
        public readonly struct ChainSegmentEqualityComparer : IEqualityComparer<ChainSegment>
        {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(ChainSegment x, ChainSegment y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                
                return x._hashCode == y._hashCode && x.IsThis == y.IsThis && x.LowerInvariant == y.LowerInvariant;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int GetHashCode(ChainSegment obj) => obj._hashCode;
        }
    }
}