using System.Runtime.CompilerServices;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet
{
    public readonly struct HelperOptions : IHelperOptions
    {
        public BindingContext Frame { get; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HelperOptions(BindingContext frame) => Frame = frame;
        
        public DataValues Data => new DataValues(Frame);
    }
}