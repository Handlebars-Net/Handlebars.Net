using System;

namespace HandlebarsDotNet.Pools
{
    internal class GenericObjectPool<T> : InternalObjectPool<T, GenericObjectPool<T>.Policy> 
        where T : class, new()
    {
        private static readonly Lazy<GenericObjectPool<T>> Instance = new(() => new GenericObjectPool<T>());
        public static GenericObjectPool<T> Shared => Instance.Value;
        
        private GenericObjectPool() : base(new Policy()) { }
        
        public readonly struct Policy : IInternalObjectPoolPolicy<T>
        {
            public T Create() => new ();
            public bool Return(T item) => true;
        }
    }
}