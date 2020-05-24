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
        public object[] Store = new object[0];

        /// <summary>
        /// Index for the next item reference
        /// </summary>
        public int CurrentIndex => _inner?.Count ?? -1;
        
        //public object[] Store => _store;
        
        /// <summary>
        /// Adds value to store
        /// </summary>
        /// <param name="key"></param>
        public object this[int key]
        {
            set
            {
                if(value == null) return;
                _inner?.Add(value);
                _objectSet?.Add(value, key);
            }
        }

        /// <summary>
        /// Uses reverse index to lookup for object key in storage using it's value
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool TryGetKeyByValue(object obj, out int key)
        {
            key = -1;
            if (obj != null) return _objectSet?.TryGetValue(obj, out key) ?? false;
            
            return false;
        }

        internal void Build()
        {
            if(_inner == null) return;
            
            Array.Resize(ref Store, _inner.Count);
            _inner.CopyTo(Store, 0);
            
            _inner.Clear();
            _inner = null;
            _objectSet?.Clear();
            _objectSet = null;
        }
    }
}