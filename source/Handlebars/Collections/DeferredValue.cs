using System;

namespace HandlebarsDotNet.Collections
{
    internal struct DeferredValue<T>
    {
        private T _value;
        private bool _isValueCreated;
        
        public Func<T> Factory { get; set; }

        public T Value
        {
            get
            {
                if (_isValueCreated) return _value;
                
                _value = Factory();
                _isValueCreated = true;
                return _value;
            }
        }
    }
}