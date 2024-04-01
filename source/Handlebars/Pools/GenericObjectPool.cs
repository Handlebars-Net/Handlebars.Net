namespace HandlebarsDotNet.Pools
{
    internal class GenericObjectPool<T> : InternalObjectPool<T, GenericObjectPool<T>.Policy> 
        where T : class, new()
    {
        public static GenericObjectPool<T> Shared { get; } = new();

        private GenericObjectPool() : base(new Policy()) { }
        
        public readonly struct Policy : IInternalObjectPoolPolicy<T>
        {
            public T Create() => new ();
            public bool Return(T item) => true;
        }
    }
}