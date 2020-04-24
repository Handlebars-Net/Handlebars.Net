using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace HandlebarsDotNet
{
    internal static class ObjectExtensions
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T As<T>(this object source) => (T) source;
    }
    
    internal static class ReadWriteLockExtensions
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DisposableContainer<ReaderWriterLockSlim> UseRead(this ReaderWriterLockSlim @lock)
        {
            @lock.EnterReadLock();
            return new DisposableContainer<ReaderWriterLockSlim>(@lock, self => self.ExitReadLock());
        }
        
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DisposableContainer<ReaderWriterLockSlim> UseWrite(this ReaderWriterLockSlim @lock)
        {
            @lock.EnterWriteLock();
            return new DisposableContainer<ReaderWriterLockSlim>(@lock, self => self.ExitWriteLock());
        }
    }
}