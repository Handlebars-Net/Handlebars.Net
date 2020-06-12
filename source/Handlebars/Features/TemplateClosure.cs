using System;
using System.Collections.Generic;

namespace HandlebarsDotNet.Features
{
    /// <summary>
    /// Used by <see cref="ClosureFeature"/> to store compiled lambda closure
    /// </summary>
    public sealed class TemplateClosure
    {
        private Dictionary<object, int> _objectSet = new Dictionary<object, int>();
        private List<object> _inner = new List<object>();

        /// <summary>
        /// Actual closure storage
        /// </summary>
        public object[] Store { get; private set; }

        internal int CurrentIndex => _inner?.Count ?? -1;
        
        internal object this[int key]
        {
            set
            {
                if(value == null) return;
                _inner?.Add(value);
                _objectSet?.Add(value, key);
            }
        }
        
        internal bool TryGetKeyByValue(object obj, out int key)
        {
            key = -1;
            if (obj != null) return _objectSet?.TryGetValue(obj, out key) ?? false;
            
            return false;
        }

        internal void Build()
        {
            if(_inner == null) return;
            
            Store = new object[_inner.Count];
            _inner.CopyTo(Store, 0);
            
            _inner.Clear();
            _inner = null;
            _objectSet?.Clear();
            _objectSet = null;
        }
    }
}