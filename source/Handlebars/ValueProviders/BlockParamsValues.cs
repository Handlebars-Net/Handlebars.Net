using System.Runtime.CompilerServices;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.ValueProviders
{
    public readonly ref struct BlockParamsValues
    {
        private readonly ChainSegment[] _variables;
        private readonly FixedSizeDictionary<ChainSegment, object, ChainSegment.ChainSegmentEqualityComparer> _values;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockParamsValues(BindingContext context, ChainSegment[] variables)
        {
            _variables = variables;
            _values = context?.BlockParamsObject;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CreateProperty(in int variableIndex, out EntryIndex<ChainSegment> index)
        {
            var variable = GetVariable(variableIndex);
            if (ReferenceEquals(variable, null))
            {
                index = new EntryIndex<ChainSegment>(-1, 0, null);
                return;
            }
            var value = UndefinedBindingResult.Create(variable);
            
            _values.AddOrReplace(variable, value, out index);
        }
        
        public object this[in EntryIndex<ChainSegment> index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if(_values == null) return;
                _values[index] = value;
            }
        }
        
        public object this[int variableIndex]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if(_values == null) return;
                var variable = GetVariable(variableIndex);
                if (ReferenceEquals(variable, null)) return;
                _values.AddOrReplace(variable, value, out _);
            }
        }

        private ChainSegment GetVariable(int index)
        {
            if (_variables == null || _variables.Length == 0 || index >= _variables.Length) return null;
            return _variables[index];
        }
    }
}