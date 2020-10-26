using System.Runtime.CompilerServices;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.ValueProviders
{
    public readonly ref struct DataValues
    {
        private readonly BindingContext _context;
        private readonly EntryIndex<ChainSegment>[] _wellKnownVariables;
        private readonly FixedSizeDictionary<ChainSegment, object, ChainSegment.ChainSegmentEqualityComparer> _data;
        
        private ICompiledHandlebarsConfiguration Configuration
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _context.Configuration;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataValues(BindingContext context)
        {
            _context = context;
            _data = _context.ContextDataObject;
            _wellKnownVariables = context.WellKnownVariables;
        }

        public object this[ChainSegment segment]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var knownVariableIndex = (int) segment.WellKnownVariable;
                if (segment.WellKnownVariable != WellKnownVariable.None && _wellKnownVariables[knownVariableIndex].IsNotEmpty)
                {
                    return _data[_wellKnownVariables[knownVariableIndex]];
                }
                
                return _data.TryGetValue(segment, out var value) ? value : null;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                var knownVariableIndex = (int) segment.WellKnownVariable;
                if (segment.WellKnownVariable != WellKnownVariable.None)
                {
                    _data.AddOrReplace(segment, value, out _wellKnownVariables[knownVariableIndex]);
                    return;
                }
                
                _data.AddOrReplace(segment, value, out _);
            }
        }

        public object this[in EntryIndex<ChainSegment> entryIndex]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _data[entryIndex];
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _data[entryIndex] = value;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CreateProperty(ChainSegment variable, out EntryIndex<ChainSegment> index)
        {
            var value = UndefinedBindingResult.Create(variable);
            _data.AddOrReplace(variable, value, out index);
            
            if (variable.WellKnownVariable != WellKnownVariable.None)
            {
                _wellKnownVariables[(int) variable.WellKnownVariable] = index;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CreateProperty(ChainSegment variable, object defaultValue, out EntryIndex<ChainSegment> index)
        {
            _data.AddOrReplace(variable, defaultValue, out index);
            
            if (variable.WellKnownVariable != WellKnownVariable.None)
            {
                _wellKnownVariables[(int) variable.WellKnownVariable] = index;
            }
        }
    }
}