using System.Threading;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet
{
    internal static class ReaderWriterLockSlimExtensions
    {
        public static DisposableContainer<ReaderWriterLockSlim> ReadLock(this ReaderWriterLockSlim @lock)
        {
            @lock.EnterReadLock();
            return new DisposableContainer<ReaderWriterLockSlim>(@lock, l => l.ExitReadLock());
        }
        
        public static DisposableContainer<ReaderWriterLockSlim> WriteLock(this ReaderWriterLockSlim @lock)
        {
            @lock.EnterWriteLock();
            return new DisposableContainer<ReaderWriterLockSlim>(@lock, l => l.ExitWriteLock());
        }
    }
}