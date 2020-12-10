using System.Runtime.CompilerServices;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet
{
    public interface IHelperOptions
    {
        BindingContext Frame { get; }
        DataValues Data { get; }
        PathInfo Name { get; }
    }

    public static class HelperOptionsExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryAccessMember<T>(this T helperOptions, object instance, ChainSegment chainSegment, out object value)
            where T: IHelperOptions
        {
            return PathResolver.TryAccessMember(helperOptions.Frame, instance, chainSegment, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ResolvePath<T>(this T helperOptions, PathInfo pathInfo)
            where T: IHelperOptions
        {
            return PathResolver.ResolvePath(helperOptions.Frame, pathInfo);
        }
    }
}