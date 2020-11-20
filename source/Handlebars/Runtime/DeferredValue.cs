using System;
using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.Runtime
{
    public sealed class DeferredValue<TState, T>
    {
        private readonly TState _state;
        private readonly Func<TState, T> _factory;
        
        private T _value;
        private bool _isValueCreated;

        public DeferredValue(TState state, Func<TState, T> factory)
        {
            _state = state;
            _factory = factory;
        }

        public override string ToString()
        {
            if (!_isValueCreated) return "Not yet created";
            return _value.ToString();
        }

        public T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_isValueCreated) return _value;
                
                _value = _factory(_state);
                _isValueCreated = true;
                return _value;
            }
        }

        public void Reset() => _isValueCreated = false;
    }
}