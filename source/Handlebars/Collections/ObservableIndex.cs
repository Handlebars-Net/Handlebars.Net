using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet.Collections
{
    [DebuggerDisplay("Count = {Count}")]
    internal class ObservableIndex<TKey, TValue, TComparer> : 
        IObservable<ObservableEvent<TValue>>,
        IObserver<ObservableEvent<TValue>>,
        IIndexed<TKey, TValue>
        where TComparer: IEqualityComparer<TKey>
    {
        private readonly ReaderWriterLockSlim _observersLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private readonly ReaderWriterLockSlim _itemsLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        
        private readonly List<IObserver<ObservableEvent<TValue>>> _observers;
        private readonly DictionarySlim<TKey, TValue, TComparer> _inner;

        public ObservableIndex(TComparer comparer, IReadOnlyIndexed<TKey, TValue> outer = null)
        {
            _inner = outer != null ? new DictionarySlim<TKey, TValue, TComparer>(outer, comparer) : new DictionarySlim<TKey, TValue, TComparer>(comparer);
            _observers = new List<IObserver<ObservableEvent<TValue>>>();
            if (outer is IObservable<ObservableEvent<TValue>> observableDictionary)
            {
                observableDictionary.Subscribe(this);
            }
        }
        
        public IDisposable Subscribe(IObserver<ObservableEvent<TValue>> observer)
        {
            using (_observersLock.WriteLock())
            {
                _observers.Add(observer);
            }

            var disposableContainer = new DisposableContainer<List<IObserver<ObservableEvent<TValue>>>, ReaderWriterLockSlim>(
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

        private void Publish(ObservableEvent<TValue> @event)
        {
            using (_observersLock.ReadLock())
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
        
        public void AddOrReplace(in TKey key, in TValue value)
        {
            using (_itemsLock.WriteLock())
            {
                _inner.AddOrReplace(key, value);
            }
            
            Publish(new DictionaryAddedObservableEvent<TKey, TValue>(key, value));
        }

        public bool ContainsKey(in TKey key)
        {
            using (_itemsLock.ReadLock())
            {
                return _inner.ContainsKey(key);
            }
        }
        
        public bool TryGetValue(in TKey key, out TValue value)
        {
            using (_itemsLock.ReadLock())
            {
                return _inner.TryGetValue(key, out value);
            }
        }

        public TValue this[in TKey key]
        {
            get => TryGetValue(key, out var value) ? value : default;
            set => AddOrReplace(key, value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            KeyValuePair<TKey, TValue>[] array; 
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
                case DictionaryAddedObservableEvent<TKey, TValue> addedObservableEvent:
                    AddOrReplace(addedObservableEvent.Key, addedObservableEvent.Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }

    internal class DictionaryAddedObservableEvent<TKey, TValue> : ObservableEvent<TValue>
    {
        public TKey Key { get; }

        public DictionaryAddedObservableEvent(TKey key, TValue value) : base(value) => Key = key;
    }

    internal abstract class ObservableEvent<T>
    {
        public T Value { get; }

        public ObservableEvent(T value) => Value = value;
    }
}