using System.Runtime.CompilerServices;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.ValueProviders
{
    public readonly ref struct BlockParamsValues
    {
        private readonly ChainSegment[] _variables;
        private readonly FixedSizeDictionary<ChainSegment, object, ChainSegment.ChainSegmentEqualityComparer> _values;
        private readonly ICompiledHandlebarsConfiguration _configuration;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockParamsValues(BindingContext context, ChainSegment[] variables)
        {
            _variables = variables;
            if (context != null)
            {
                _values = context.BlockParamsObject;
                _configuration = context.Configuration;   
            }
            else
            {
                _values = null;
                _configuration = null;
            }
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

        private ChainSegment GetVariable(int index)
        {
            if (_variables == null || _variables.Length == 0 || index >= _variables.Length) return null;
            return _variables[index];
        }
    }
}