using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet.Collections
{
    [DebuggerDisplay("Count = {Count}")]
    public class ObservableList<T> : 
        IAppendOnlyList<T>, 
        IObservable<IObservableEvent<T>>, 
        IObserver<IObservableEvent<T>>
    {
        private readonly ReaderWriterLockSlim _observersLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private readonly ReaderWriterLockSlim _itemsLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        
        private readonly WeakCollection<IObserver<IObservableEvent<T>>> _observers = new WeakCollection<IObserver<IObservableEvent<T>>>();
        private readonly List<T> _inner;

        public ObservableList(IEnumerable<T>? list = null)
        {
            _inner = list != null ? new List<T>(list) : new List<T>();
            if (list is IObservable<IObservableEvent<T>> observableList)
            {
                observableList.Subscribe(this);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            T[] array;
            using (_itemsLock.ReadLock())
            {
                array = _inner.ToArray();
            }

            for (int index = 0; index < array.Length; index++)
            {
                yield return array[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T value)
        {
            using (_itemsLock.WriteLock())
            {
                _inner.Add(value);
            }
            
            Publish(new AddedObservableEvent<T>(value));
        }
        
        public int Count
        {
            get
            {
                using (_itemsLock.ReadLock())
                {
                    return _inner.Count;
                }
            }
        }
        
        public T this[int index]
        {
            get
            {
                using (_itemsLock.ReadLock())
                {
                    return _inner[index];
                }
            }
        }

        public IDisposable Subscribe(IObserver<IObservableEvent<T>> observer)
        {
            using (_observersLock.WriteLock())
            {
                _observers.Add(observer);
            }
    
            var disposableContainer = new DisposableContainer<WeakCollection<IObserver<IObservableEvent<T>>>, ReaderWriterLockSlim>(
                _observers, _observersLock, (observers, @lock) =>
                {
                    using (@lock.WriteLock())
                    {
                        observers.Remove(this);
                    }
                }
            );
            
            return disposableContainer;
        }

        public void OnCompleted()
        {
            // nothing to do here
        }

        public void OnError(Exception error)
        {
            // nothing to do here
        }

        public void OnNext(IObservableEvent<T> value)
        {
            switch (value)
            {
                case AddedObservableEvent<T> addedObservableEvent:
                    Add(addedObservableEvent.Value);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
        
        private void Publish(IObservableEvent<T> @event)
        {
            using (_observersLock.ReadLock())
            {
                foreach (var observer in _observers)
                {
                    try
                    {
                        observer.OnNext(@event);
                    }
                    catch
                    {
                        // ignore
                    }
                }
            }
        }
    }

    public class AddedObservableEvent<T> : ObservableEvent<T>
    {
        public AddedObservableEvent(T value) : base(value)
        {
        }
    }
}