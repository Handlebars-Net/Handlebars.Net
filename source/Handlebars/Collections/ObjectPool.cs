using System;
using System.Collections.Concurrent;
using System.Threading;

namespace HandlebarsDotNet
{
    internal static class ObjectPoolExtensions
    {
        public static DisposableContainer<T> Use<T>(this InternalObjectPool<T> objectPool) where T : class
        {
            return new DisposableContainer<T>(objectPool.Get(), objectPool.Return);
        }
    }

    internal class InternalObjectPool<T>
        where T: class
    {
        private readonly IInternalObjectPoolPolicy<T> _policy;
        private ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        private T _firstItem;
    
        public InternalObjectPool(IInternalObjectPoolPolicy<T> policy)
        {
            Handlebars.Disposables.Enqueue(new Disposer(this));
            
            _policy = policy;
    
            for (var i = 0; i < 5; i++) Return(_policy.Create());
        }
    
        public T Get()
        {
            var item = _firstItem;
            if (item == null || item != Interlocked.CompareExchange(ref _firstItem, null, item))
            {
                if (_queue.TryDequeue(out item))
                {
                    return item;
                }
    
                item = _policy.Create();
            }
            
            return item;
        }
    
        public void Return(T obj)
        {
            if (!_policy.Return(obj)) return;
            
            if (_firstItem == null)
            {
                // Intentionally not using interlocked here. 
                // In a worst case scenario two objects may be stored into same slot.
                // It is very unlikely to happen and will only mean that one of the objects will get collected.
                _firstItem = obj;
                return;
            }
            
            _queue.Enqueue(obj);
        }
    
        private sealed class Disposer : IDisposable
        {
            private readonly InternalObjectPool<T> _target;
    
            public Disposer(InternalObjectPool<T> target) => _target = target;

            public void Dispose() => _target._queue = new ConcurrentQueue<T>();
        }
    }

    internal interface IInternalObjectPoolPolicy<T>
    {
        T Create();
        bool Return(T item);
    }
}