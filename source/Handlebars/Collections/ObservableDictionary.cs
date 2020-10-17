using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HandlebarsDotNet.Collections
{
    internal class ObservableDictionary<TKey, TValue> : IObservable<ObservableEvent<TValue>>, IDictionary<TKey, TValue>, IObserver<ObservableEvent<TValue>>
    {
        private readonly HashedCollection<IObserver<ObservableEvent<TValue>>> _observers;
        private readonly Dictionary<TKey, TValue> _inner;

        public ObservableDictionary(IDictionary<TKey, TValue> outer = null, IEqualityComparer<TKey> comparer = null)
        {
            comparer ??= EqualityComparer<TKey>.Default;
            _inner = outer != null ? new Dictionary<TKey, TValue>(outer, comparer) : new Dictionary<TKey, TValue>(comparer);
            _observers = new HashedCollection<IObserver<ObservableEvent<TValue>>>();
            if (outer is ObservableDictionary<TKey, TValue> observableDictionary)
            {
                observableDictionary.Subscribe(this);
            }
        }
        
        public IDisposable Subscribe(IObserver<ObservableEvent<TValue>> observer)
        {
            lock (_observers)
            {
                _observers.Add(observer);
                return new UnsubscribeEvent<TValue>(_observers, observer);
            }
        }

        private void Publish(ObservableEvent<TValue> @event)
        {
            lock (_observers)
            {
                for (var index = 0; index < _observers.Count; index++)
                {
                    try
                    {
                        _observers[index].OnNext(@event);
                    }
                    catch
                    {
                        // ignore
                    }
                }
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            lock (_inner)
            {
                Publish(new AddedObservableEvent(item.Key, item.Value));
                _inner.Add(item.Key, item.Value);
            }
        }

        public void Clear()
        {
            lock (_inner)
            {
                foreach (var pair in _inner)
                {
                    Publish(new RemovedObservableEvent(pair.Key, pair.Value));
                }
                
                _inner.Clear();
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            lock (_inner)
            {
                return _inner.As<ICollection<KeyValuePair<TKey, TValue>>>().Contains(item);
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock (_inner)
            {
                _inner.As<ICollection<KeyValuePair<TKey, TValue>>>().CopyTo(array, arrayIndex);
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            lock (_inner)
            {
                var removed = _inner.As<ICollection<KeyValuePair<TKey, TValue>>>().Remove(item);
                if (removed)
                {
                    Publish(new RemovedObservableEvent(item.Key, item.Value));
                }

                return removed;
            }
        }

        public int Count
        {
            get
            {
                lock (_inner)
                {
                    return _inner.Count;
                }
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly { get; } = false;
        
        public void Add(TKey key, TValue value)
        {
            lock (_inner)
            {
                _inner.Add(key, value);
            }
            
            Publish(new AddedObservableEvent(key, value));
        }

        public bool ContainsKey(TKey key)
        {
            lock (_inner)
            {
                return _inner.ContainsKey(key);
            }
        }

        public bool Remove(TKey key)
        {
            TValue value;
            lock (_inner)
            {
                if (!_inner.TryGetValue(key, out value)) return false;
                _inner.Remove(key);
            }

            Publish(new RemovedObservableEvent(key, value));
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_inner)
            {
                return _inner.TryGetValue(key, out value);
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (_inner)
                {
                    return _inner[key];
                }
            }
            set
            {
                lock (_inner)
                {
                    if (_inner.TryGetValue(key, out var oldValue))
                    {
                        Publish(new ReplacedObservableEvent(key, oldValue, value));
                        _inner[key] = value;
                    }
                    else
                    {
                        Add(key, value);
                    }
                }
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                lock (_inner)
                {
                    return _inner.Keys.ToList();
                }
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                lock (_inner)
                {
                    return _inner.Values.ToList();
                }
            }
        }
        
        internal class AddedObservableEvent : ObservableEvent<TValue>
        {
            public TKey Key { get; }

            public AddedObservableEvent(TKey key, TValue value) : base(value)
            {
                Key = key;
            }
        }
    
        internal class RemovedObservableEvent : ObservableEvent<TValue>
        {
            public TKey Key { get; }

            public RemovedObservableEvent(TKey key, TValue value) : base(value)
            {
                Key = key;
            }
        }
    
        internal class ReplacedObservableEvent : ObservableEvent<TValue>
        {
            public TKey Key { get; }
            public TValue OldValue { get; }

            public ReplacedObservableEvent(TKey key, TValue oldValue, TValue value) : base(value)
            {
                Key = key;
                OldValue = oldValue;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            KeyValuePair<TKey, TValue>[] array; 
            lock (_inner)
            {
                array = _inner.ToArray();
            }

            for (int index = 0; index < array.Length; index++)
            {
                yield return array[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(ObservableEvent<TValue> value)
        {
            switch (value)
            {
                case AddedObservableEvent addedObservableEvent:
                    Add(addedObservableEvent.Key, addedObservableEvent.Value);
                    break;
                case RemovedObservableEvent removedObservableEvent:
                    Remove(removedObservableEvent.Key);
                    break;
                case ReplacedObservableEvent replacedObservableEvent:
                    this[replacedObservableEvent.Key] = replacedObservableEvent.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
    
    internal abstract class ObservableEvent<T>
    {
        public T Value { get; }

        public ObservableEvent(T value)
        {
            Value = value;
        }
    }
    
    internal class UnsubscribeEvent<T> : IDisposable
    {
        private readonly ICollection<IObserver<ObservableEvent<T>>> _observers;
        private readonly IObserver<ObservableEvent<T>> _observer;

        public UnsubscribeEvent(ICollection<IObserver<ObservableEvent<T>>> observers, IObserver<ObservableEvent<T>> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            _observers.Remove(_observer);
            GC.SuppressFinalize(this);
        }
    }
}