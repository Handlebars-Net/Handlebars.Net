using System;

namespace HandlebarsDotNet.Runtime
{
    public sealed class GcDeferredValue<TState, T>
        where T : class
    {
        private readonly TState _state;
        private readonly Func<TState, T> _factory;
        
        private WeakReference<T>? _value;

        public GcDeferredValue(TState state, Func<TState, T> factory)
        {
            _state = state;
            _factory = factory;
        }

        public override string? ToString()
        {
            if (_value == null) return "Not yet created";
            if (!_value.TryGetTarget(out var value)) return "GC collected";
            
            return value.ToString();
        }

        public T Value
        {
            get
            {
                T value;
                if (_value != null)
                {
                    if(_value.TryGetTarget(out value!)) return value;

                    value = _factory(_state);
                    _value.SetTarget(value);
                }
                else
                {
                    value = _factory(_state);
                    _value = new WeakReference<T>(value);
                }
                
                return value;
            }
        }
    }
}