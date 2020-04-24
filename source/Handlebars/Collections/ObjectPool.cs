using Microsoft.Extensions.ObjectPool;

namespace HandlebarsDotNet
{
    internal static class ObjectPoolExtensions
    {
        public static DisposableContainer<T> Use<T>(this ObjectPool<T> objectPool) where T : class
        {
            return new DisposableContainer<T>(objectPool.Get(), objectPool.Return);
        }
    }
}