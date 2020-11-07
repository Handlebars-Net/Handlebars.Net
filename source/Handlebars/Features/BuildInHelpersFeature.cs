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
        public void OnCompiling(ICompiledHandlebarsConfiguration configuration)
        {
            configuration.BlockHelpers["with"] = new Ref<IHelperDescriptor<BlockHelperOptions>>(new WithBlockHelperDescriptor());
            configuration.BlockHelpers["*inline"] = new Ref<IHelperDescriptor<BlockHelperOptions>>(new InlineBlockHelperDescriptor());
            configuration.Helpers["lookup"] = new Ref<IHelperDescriptor<HelperOptions>>(new LookupReturnHelperDescriptor());
        }

        public void CompilationCompleted()
        {
            // noting to do here
        }
    }
}