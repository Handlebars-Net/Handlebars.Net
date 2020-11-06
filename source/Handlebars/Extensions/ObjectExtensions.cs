using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HandlebarsDotNet
{
    internal static class ObjectExtensions
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T As<T>(this object source) => (T) source;
    }
}