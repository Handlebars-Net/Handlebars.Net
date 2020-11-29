using System.Runtime.CompilerServices;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet
{
    public readonly struct HelperOptions : IHelperOptions
    {
        public BindingContext Frame { get; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HelperOptions(PathInfo name, BindingContext frame)
        {
            Frame = frame;
            Name = name;
        }

        public DataValues Data => new DataValues(Frame);
        
        public PathInfo Name { get; }
    }
}