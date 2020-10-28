using System.Runtime.CompilerServices;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet
{
    public readonly struct HelperOptions
    {
        public readonly BindingContext Frame;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HelperOptions(BindingContext frame) => Frame = frame;
        
        public DataValues Data => new DataValues(Frame);
    }
}