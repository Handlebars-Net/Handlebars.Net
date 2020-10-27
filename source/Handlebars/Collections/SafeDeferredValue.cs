using System;

namespace HandlebarsDotNet.Collections
{
    internal class SafeDeferredValue<TState, T>
    {
        private readonly object _lock = new object();
        
        private readonly TState _state;
        private readonly Func<TState, T> _factory;

        private T _value;
        private volatile bool _isValueCreated;

        public SafeDeferredValue(TState state, Func<TState, T> factory)
        {
            _state = state;
            _factory = factory;
        }
        
        public T Value
        {
            get
            {
                if (_isValueCreated) return _value;

                lock (_lock)
                {
                    if (_isValueCreated) return _value;
                    
                    _value = _factory(_state);
                    _isValueCreated = true;
                    return _value;   
                }
            }
        }

        public void Reset() => _isValueCreated = false;
    }
}