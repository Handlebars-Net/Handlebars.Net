using System.Runtime.CompilerServices;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet.Pools
{
    internal static class ObjectPoolExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DisposableContainer<T, InternalObjectPool<T, TPolicy>> Use<T, TPolicy>(this InternalObjectPool<T, TPolicy> objectPool) 
            where T : class where TPolicy : IInternalObjectPoolPolicy<T>
        {
            return new DisposableContainer<T, InternalObjectPool<T, TPolicy>>(
                objectPool.Get(), 
                objectPool, 
                (item, pool) => pool.Return(item)
            );
        }
    }
}