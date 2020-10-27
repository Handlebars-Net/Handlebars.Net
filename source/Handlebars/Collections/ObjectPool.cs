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
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

        public InternalObjectPool(IInternalObjectPoolPolicy<T> policy)
        {
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
    }

    internal interface IInternalObjectPoolPolicy<T>
    {
        T Create();
        bool Return(T item);
    }
}