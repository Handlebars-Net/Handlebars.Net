using System.Collections.Concurrent;

namespace HandlebarsDotNet
{
    internal abstract class ObjectPool<T>
    {
        private readonly ConcurrentQueue<T> _objects;

        protected ObjectPool()
        {
            _objects = new ConcurrentQueue<T>();
        }

        public T GetObject()
        {
            return !_objects.TryDequeue(out var item)
                ? CreateObject()
                : item;
        }

        public DisposableContainer<T> Use()
        {
            return new DisposableContainer<T>(GetObject(), PutObject);
        }

        public virtual void PutObject(T item)
        {
            _objects.Enqueue(item);
        }

        protected abstract T CreateObject();
    }
}