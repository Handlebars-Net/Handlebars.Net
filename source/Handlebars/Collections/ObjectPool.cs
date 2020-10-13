using System;
using System.Collections.Concurrent;

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

        public InternalObjectPool(IInternalObjectPoolPolicy<T> policy)
        {
            Handlebars.Disposables.Enqueue(new Disposer(this));
            
            _policy = policy;
    
            for (var i = 0; i < 5; i++) Return(_policy.Create());
        }
    
        public T Get()
        {
            if (_queue.TryDequeue(out var item))
            {
                return item;
            }
    
            return _policy.Create();
        }
    
        public void Return(T obj)
        {
            if (!_policy.Return(obj)) return;
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