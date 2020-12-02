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
        private static readonly InlineBlockHelperDescriptor InlineBlockHelperDescriptor = new InlineBlockHelperDescriptor();
        private static readonly LookupReturnHelperDescriptor LookupReturnHelperDescriptor = new LookupReturnHelperDescriptor();

        public void OnCompiling(ICompiledHandlebarsConfiguration configuration)
        {
            configuration.BlockHelpers["with"] = new Ref<IHelperDescriptor<BlockHelperOptions>>(WithBlockHelperDescriptor);
            configuration.BlockHelpers["*inline"] = new Ref<IHelperDescriptor<BlockHelperOptions>>(InlineBlockHelperDescriptor);
            configuration.Helpers["lookup"] = new Ref<IHelperDescriptor<HelperOptions>>(LookupReturnHelperDescriptor);
        }

        public void CompilationCompleted()
        {
            // noting to do here
        }
    }
}