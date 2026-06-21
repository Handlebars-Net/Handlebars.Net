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
            @lock.EnterReadLock(); // NOSONAR S7133 — lock released in DisposableContainer.Dispose via `using`
            return new DisposableContainer<ReaderWriterLockSlim>(@lock, l => l.ExitReadLock());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DisposableContainer<ReaderWriterLockSlim> WriteLock(this ReaderWriterLockSlim @lock)
        {
            @lock.EnterWriteLock(); // NOSONAR S7133 — lock released in DisposableContainer.Dispose via `using`
            return new DisposableContainer<ReaderWriterLockSlim>(@lock, l => l.ExitWriteLock());
        }
    }
}