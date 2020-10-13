using System;
using System.Collections;
using System.Collections.Generic;

namespace HandlebarsDotNet.Collections
{
    internal class ObservableList<T> : IList<T>, IReadOnlyList<T>, IObservable<ObservableEvent<T>>, IObserver<ObservableEvent<T>>
    {
        private readonly object _lock = new object();
        
        private readonly List<IObserver<ObservableEvent<T>>> _observers = new List<IObserver<ObservableEvent<T>>>();
        private readonly List<T> _inner;

        public ObservableList(ICollection<T> list = null)
        {
            _inner = list != null ? new List<T>(list) : new List<T>();
            if (list is ObservableList<T> observableList)
            {
                observableList.Subscribe(this);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            T[] array;
            lock (_lock)
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
            lock (_lock)
            {
                return ((IEnumerable) _inner).GetEnumerator();
            }
        }

        public void Add(T item)
        {
            lock (_lock)
            {
                _inner.Add(item);
            }
            
            Publish(new AddedObservableEvent(item));
        }

        public void Clear()
        {
            T[] array;
            lock (_lock)
            {
                array = _inner.ToArray();
                _inner.Clear();
            }

            for (var index = 0; index < array.Length; index++)
            {
                var item = array[index];
                Publish(new RemovedObservableEvent(item));
            }
        }

        public bool Contains(T item)
        {
            lock (_lock)
            {
                return _inner.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_lock)
            {
                _inner.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            bool removed;
            lock (_lock)
            {
                removed = _inner.Remove(item);
            }

            if (removed)
            {
                Publish(new RemovedObservableEvent(item));
            }

            return removed;
        }

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _inner.Count;
                }
            }
        }

        public bool IsReadOnly { get; } = false;

        public int IndexOf(T item)
        {
            lock (_lock)
            {
                return _inner.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (_lock)
            {
                _inner.Insert(index, item);
            }
            
            Publish(new InsertObservableEvent(item, index));
        }

        public void RemoveAt(int index)
        {
            T value;
            lock (_lock)
            {
                value = _inner[index];
                _inner.RemoveAt(index);
            }
            
            Publish(new RemovedObservableEvent(value, index));
        }

        public T this[int index]
        {
            get
            {
                lock (_lock)
                {
                    return _inner[index];
                }
            }
            set
            {
                T currentValue;
                lock (_lock)
                {
                    currentValue = _inner[index];
                    _inner[index] = value;
                }

                Publish(new ReplacedObservableEvent(value, currentValue, index));
            }
        }

        public IDisposable Subscribe(IObserver<ObservableEvent<T>> observer)
        {
            _observers.Add(observer);
            return new UnsubscribeEvent<T>(_observers, observer);
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(ObservableEvent<T> value)
        {
            switch (value)
            {
                case AddedObservableEvent addedObservableEvent:
                    Add(addedObservableEvent.Value);
                    break;
                case RemovedObservableEvent removedObservableEvent when !removedObservableEvent.Index.HasValue:
                    Remove(removedObservableEvent.Value);
                    break;
                case RemovedObservableEvent removedObservableEvent when removedObservableEvent.Index.HasValue:
                    RemoveAt(removedObservableEvent.Index.Value);
                    break;
                case ReplacedObservableEvent replacedObservableEvent:
                    this[replacedObservableEvent.Index] = replacedObservableEvent.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
        
        private void Publish(ObservableEvent<T> @event)
        {
            for (int index = 0; index < _observers.Count; index++)
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

        internal class AddedObservableEvent : ObservableEvent<T>
        {
            public AddedObservableEvent(T value) : base(value)
            {
            }
        }
        
        internal class InsertObservableEvent : ObservableEvent<T>
        {
            public int Index { get; }

            public InsertObservableEvent(T value, int index) : base(value)
            {
                Index = index;
            }
        }
        
        internal class ReplacedObservableEvent : ObservableEvent<T>
        {
            public T OldValue { get; }
            public int Index { get; }

            public ReplacedObservableEvent(T value, T oldValue, int index) : base(value)
            {
                OldValue = oldValue;
                Index = index;
            }
        }

        internal class RemovedObservableEvent : ObservableEvent<T>
        {
            public int? Index { get; }

            public RemovedObservableEvent(T value, int? index = null) : base(value)
            {
                Index = index;
            }
        }
    }
}