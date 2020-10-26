using System.Runtime.CompilerServices;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.ValueProviders
{
    public readonly ref struct ObjectIteratorValues
    {
        private readonly FixedSizeDictionary<ChainSegment, object, ChainSegment.ChainSegmentEqualityComparer> _data;
        private readonly bool _supportLastInObjectIterations;
        
        private readonly EntryIndex<ChainSegment>[] _wellKnownVariables;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectIteratorValues(BindingContext bindingContext) : this()
        {
            var configuration = bindingContext.Configuration;
            
            _data = bindingContext.ContextDataObject;
            _supportLastInObjectIterations = configuration.Compatibility.SupportLastInObjectIterations;
            _wellKnownVariables = bindingContext.WellKnownVariables;
            if (!_supportLastInObjectIterations)
            {
                var undefined = UndefinedBindingResult.Create(ChainSegment.Last);
                _data.AddOrReplace(ChainSegment.Last, undefined, out _wellKnownVariables[(int) ChainSegment.Last.WellKnownVariable]);
            }
            else
            {
                _data.AddOrReplace(ChainSegment.Last, BoxedValues.False, out _wellKnownVariables[(int) ChainSegment.Last.WellKnownVariable]);
            }
            
            _data.AddOrReplace(ChainSegment.Key, null, out _wellKnownVariables[(int) ChainSegment.Key.WellKnownVariable]);
            _data.AddOrReplace(ChainSegment.Value, null, out _wellKnownVariables[(int) ChainSegment.Value.WellKnownVariable]);
            _data.AddOrReplace(ChainSegment.First, BoxedValues.True, out _wellKnownVariables[(int) ChainSegment.First.WellKnownVariable]);
            _data.AddOrReplace(ChainSegment.Index, BoxedValues.Zero, out _wellKnownVariables[(int) ChainSegment.Index.WellKnownVariable]);
        }

        public object Key
        {
            get => _data[_wellKnownVariables[(int) ChainSegment.Key.WellKnownVariable]];
            set => _data[_wellKnownVariables[(int) ChainSegment.Key.WellKnownVariable]] = value;
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
            set
            {
                if(!_supportLastInObjectIterations) return;
                _data[_wellKnownVariables[(int) ChainSegment.Last.WellKnownVariable]] = value;
            }
        }
    }
}