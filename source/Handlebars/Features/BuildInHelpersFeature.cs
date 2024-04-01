using HandlebarsDotNet.Decorators;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.BlockHelpers;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet.Features
{
    internal class BuildInHelpersFeatureFactory : IFeatureFactory
    {
        public IFeature CreateFeature() => new BuildInHelpersFeature();
    }

    [FeatureOrder(int.MinValue)]
    internal class BuildInHelpersFeature : IFeature
    {
        private static readonly WithBlockHelperDescriptor WithBlockHelperDescriptor = new WithBlockHelperDescriptor();
        private static readonly LookupReturnHelperDescriptor LookupReturnHelperDescriptor = new LookupReturnHelperDescriptor();
        private static readonly InlineBlockDecoratorDescriptor InlineBlockHelperDescriptor = new InlineBlockDecoratorDescriptor();

        public void OnCompiling(ICompiledHandlebarsConfiguration configuration)
        {
            configuration.BlockHelpers["with"] = new Ref<IHelperDescriptor<BlockHelperOptions>>(WithBlockHelperDescriptor);
            configuration.Helpers["lookup"] = new Ref<IHelperDescriptor<HelperOptions>>(LookupReturnHelperDescriptor);
            configuration.BlockDecorators["*inline"] = new Ref<IDecoratorDescriptor<BlockDecoratorOptions>>(InlineBlockHelperDescriptor);
        }

        public void CompilationCompleted()
        {
            // noting to do here
        }
    }
}