using System.Runtime.CompilerServices;
using System.Threading;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet
{
    internal static class ReaderWriterLockSlimExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DisposableContainer<ReaderWriterLockSlim> ReadLock(this ReaderWriterLockSlim @lock)
        {
            @lock.EnterReadLock();
            return new DisposableContainer<ReaderWriterLockSlim>(@lock, l => l.ExitReadLock());
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DisposableContainer<ReaderWriterLockSlim> WriteLock(this ReaderWriterLockSlim @lock)
        {
            @lock.EnterWriteLock();
            return new DisposableContainer<ReaderWriterLockSlim>(@lock, l => l.ExitWriteLock());
        }
    }
}