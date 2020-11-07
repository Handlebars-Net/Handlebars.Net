using System.Runtime.CompilerServices;
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
            configuration.BlockHelpers["with"] = new Ref<BlockHelperDescriptorBase>(new WithBlockHelperDescriptor());
            configuration.BlockHelpers["*inline"] = new Ref<BlockHelperDescriptorBase>(new InlineBlockHelperDescriptor());
            configuration.Helpers["lookup"] = new Ref<HelperDescriptorBase>(new LookupReturnHelperDescriptor(configuration));
        }

        public void CompilationCompleted()
        {
            // noting to do here
        }
    }
}