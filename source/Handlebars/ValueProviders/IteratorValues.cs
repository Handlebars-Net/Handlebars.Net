using System.Runtime.CompilerServices;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.ValueProviders
{
    public readonly ref struct IteratorValues
    {
        private readonly FixedSizeDictionary<ChainSegment, object, ChainSegment.ChainSegmentEqualityComparer> _data;
        
        private readonly EntryIndex<ChainSegment>[] _wellKnownVariables;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IteratorValues(BindingContext bindingContext) : this()
        {
            _data = bindingContext.ContextDataObject;
            _wellKnownVariables = bindingContext.WellKnownVariables;
            
            _data.AddOrReplace(ChainSegment.Value, null, out _wellKnownVariables[(int) ChainSegment.Value.WellKnownVariable]);
            _data.AddOrReplace(ChainSegment.First, BoxedValues.True, out _wellKnownVariables[(int) ChainSegment.First.WellKnownVariable]);
            _data.AddOrReplace(ChainSegment.Last, BoxedValues.False, out _wellKnownVariables[(int) ChainSegment.Last.WellKnownVariable]);
            _data.AddOrReplace(ChainSegment.Index, BoxedValues.Zero, out _wellKnownVariables[(int) ChainSegment.Index.WellKnownVariable]);
        }
        
        public object Value
        {
            get => _data[_wellKnownVariables[(int) ChainSegment.Value.WellKnownVariable]];
            set => _data[_wellKnownVariables[(int) ChainSegment.Value.WellKnownVariable]] = value;
        }

        public object First
        {
            get => _data[_wellKnownVariables[(int) ChainSegment.First.WellKnownVariable]];
            set => _data[_wellKnownVariables[(int) ChainSegment.First.WellKnownVariable]] = value;
        }

        public object Index
        {
            get => _data[_wellKnownVariables[(int) ChainSegment.Index.WellKnownVariable]];
            set => _data[_wellKnownVariables[(int) ChainSegment.Index.WellKnownVariable]] = value;
        }

        public object Last
        {
            get => _data[_wellKnownVariables[(int) ChainSegment.Last.WellKnownVariable]];
            set => _data[_wellKnownVariables[(int) ChainSegment.Last.WellKnownVariable]] = value;
        }
    }
}