using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.IO
{
    /// <summary>
    /// Tracks which <see cref="string"/> instances already went through the encoding pipeline
    /// (e.g. captured output of a subexpression helper) so they are not encoded a second time
    /// when written elsewhere. Marking is by object reference, not value, so it never affects
    /// any string a caller didn't obtain from this exact pipeline — and critically, the marked
    /// value stays a plain <see cref="string"/> the whole way through, so it round-trips safely
    /// through helper argument binding, reflection-based helpers, and any other consumer that
    /// only knows how to handle <see cref="string"/>.
    /// </summary>
    internal static class SafeStrings
    {
        private static readonly ConditionalWeakTable<string, object> Marked = new();
        private static readonly object Sentinel = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Mark(string value)
        {
            if (value.Length == 0) return value;
            Marked.GetValue(value, _ => Sentinel);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSafe(string value) => value.Length == 0 || Marked.TryGetValue(value, out _);
    }
}
