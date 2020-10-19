using System.Runtime.CompilerServices;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.BlockHelpers;

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
            var pathInfoStore = configuration.PathInfoStore;
            configuration.BlockHelpers[pathInfoStore.GetOrAdd("with")] = new StrongBox<BlockHelperDescriptorBase>(new WithBlockHelperDescriptor(configuration));
            configuration.BlockHelpers[pathInfoStore.GetOrAdd("*inline")] = new StrongBox<BlockHelperDescriptorBase>(new InlineBlockHelperDescriptor(configuration));
            configuration.Helpers[pathInfoStore.GetOrAdd("lookup")] = new StrongBox<HelperDescriptorBase>(new LookupReturnHelperDescriptor(configuration));
        }

        public void CompilationCompleted()
        {
            // noting to do here
        }
    }
}