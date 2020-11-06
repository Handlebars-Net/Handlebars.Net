using System;

namespace HandlebarsDotNet.Runtime
{
    public sealed class GcDeferredValue<TState, T>
        where T : class
    {
        private readonly TState _state;
        private readonly Func<TState, T> _factory;
        
        private WeakReference<T> _value;
        private bool _isValueCreated;

        public GcDeferredValue(TState state, Func<TState, T> factory)
        {
            _state = state;
            _factory = factory;
        }
        
        public T Value
        {
            get
            {
                T value;
                if (_isValueCreated)
                {
                    if(_value.TryGetTarget(out value)) return value;

                    value = _factory(_state);
                    _value.SetTarget(value);
                }
                else
                {
                    value = _factory(_state);
                    _value = new WeakReference<T>(value);
                    _isValueCreated = true;
                }
                
                return value;
            }
        }
    }
}