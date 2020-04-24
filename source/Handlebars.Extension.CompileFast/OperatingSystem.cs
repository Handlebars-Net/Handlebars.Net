using System.Runtime.InteropServices;

namespace HandlebarsDotNet.Extension.CompileFast
{
    internal static class OperatingSystem
    {
#if netFramework
        public static bool IsWindows() => true;
#else
        public static bool IsWindows() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif
    }
}