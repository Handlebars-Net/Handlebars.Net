using System.Runtime.CompilerServices;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Decorators;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet
{
    public readonly struct DecoratorOptions : IDecoratorOptions
    {
        public BindingContext Frame { get; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DecoratorOptions(
            PathInfo name,
            BindingContext frame
        )
        {
            Frame = frame;
            Name = name;
        }

        public DataValues Data => new DataValues(Frame);
        
        public PathInfo Name { get; }
        
        IIndexed<string, IHelperDescriptor<HelperOptions>> IHelpersRegistry.GetHelpers() => Frame.Helpers;

        IIndexed<string, IHelperDescriptor<BlockHelperOptions>> IHelpersRegistry.GetBlockHelpers() => Frame.BlockHelpers;
    }
}