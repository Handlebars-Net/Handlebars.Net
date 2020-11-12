using System.Linq;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.BlockHelpers;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet.Features
{
    public static class MissingHelperFeatureExtension
    {
        /// <summary>
        /// Allows to intercept calls to missing helpers.
        /// <para>For Handlebarsjs docs see: https://handlebarsjs.com/guide/hooks.html#helpermissing</para>
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="helperMissing">Delegate that returns interceptor for <see cref="HandlebarsReturnHelper"/> and <see cref="HandlebarsHelper"/></param>
        /// <param name="blockHelperMissing">Delegate that returns interceptor for <see cref="HandlebarsBlockHelper"/></param>
        /// <returns></returns>
        public static HandlebarsConfiguration RegisterMissingHelperHook(
            this HandlebarsConfiguration configuration,
            IHelperDescriptor<HelperOptions> helperMissing = null,
            IHelperDescriptor<BlockHelperOptions> blockHelperMissing = null
        )
        {
            var feature = new MissingHelperFeatureFactory(helperMissing, blockHelperMissing);

            var features = configuration.CompileTimeConfiguration.Features;

            features.Remove(features.OfType<MissingHelperFeatureFactory>().Single());
            
            features.Add(feature);

            return configuration;
        }
        
        /// <summary>
        /// Allows to intercept calls to missing helpers.
        /// <para>For Handlebarsjs docs see: https://handlebarsjs.com/guide/hooks.html#helpermissing</para>
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="helperMissing">Delegate that returns interceptor for <see cref="HandlebarsReturnHelper"/> and <see cref="HandlebarsHelper"/></param>
        /// <param name="blockHelperMissing">Delegate that returns interceptor for <see cref="HandlebarsBlockHelper"/></param>
        /// <returns></returns>
        public static HandlebarsConfiguration RegisterMissingHelperHook(
            this HandlebarsConfiguration configuration,
            HandlebarsReturnHelper helperMissing = null,
            HandlebarsBlockHelper blockHelperMissing = null
        )
        {
            return configuration.RegisterMissingHelperHook(
                helperMissing != null ? new DelegateReturnHelperDescriptor("helperMissing", helperMissing) : null,
                blockHelperMissing != null ? new DelegateBlockHelperDescriptor("blockHelperMissing", blockHelperMissing) : null
            );
        }
    }

    internal class MissingHelperFeatureFactory : IFeatureFactory
    {
        private readonly IHelperDescriptor<HelperOptions> _returnHelper;
        private readonly IHelperDescriptor<BlockHelperOptions> _blockHelper;

        public MissingHelperFeatureFactory(
            IHelperDescriptor<HelperOptions> returnHelper = null,
            IHelperDescriptor<BlockHelperOptions> blockHelper = null
        )
        {
            _returnHelper = returnHelper;
            _blockHelper = blockHelper;
        }

        public IFeature CreateFeature() => new MissingHelperFeature(_returnHelper, _blockHelper);
    }

    [FeatureOrder(int.MaxValue)]
    internal class MissingHelperFeature : IFeature
    {
        private const string HelperMissingKey = "helperMissing";
        private const string BlockHelperMissingKey = "blockHelperMissing";
        
        private readonly IHelperDescriptor<HelperOptions> _helper;
        private readonly IHelperDescriptor<BlockHelperOptions> _blockHelper;

        public MissingHelperFeature(
            IHelperDescriptor<HelperOptions> helper,
            IHelperDescriptor<BlockHelperOptions> blockHelper
        )
        {
            _helper = helper;
            _blockHelper = blockHelper;
        }

        public void OnCompiling(ICompiledHandlebarsConfiguration configuration)
        {
            var helperMissingPathInfo = PathInfoStore.Shared.GetOrAdd(HelperMissingKey);
            if(!configuration.Helpers.ContainsKey(helperMissingPathInfo))
            {
                var helper = _helper ?? new MissingHelperDescriptor();
                if (configuration.Helpers.TryGetValue(helperMissingPathInfo, out var existingHelper))
                {
                    existingHelper.Value = helper;
                    return;
                }
                
                configuration.Helpers.AddOrReplace(helperMissingPathInfo, new Ref<IHelperDescriptor<HelperOptions>>(helper));
            }

            var blockHelperMissingKeyPathInfo = PathInfoStore.Shared.GetOrAdd(BlockHelperMissingKey);
            if(!configuration.BlockHelpers.ContainsKey(blockHelperMissingKeyPathInfo))
            {
                var blockHelper = _blockHelper ?? new MissingBlockHelperDescriptor();
                if (configuration.BlockHelpers.TryGetValue(blockHelperMissingKeyPathInfo, out var existingHelper))
                {
                    existingHelper.Value = blockHelper;
                    return;
                }
                
                configuration.BlockHelpers.AddOrReplace(blockHelperMissingKeyPathInfo, new Ref<IHelperDescriptor<BlockHelperOptions>>(blockHelper));
            }
        }

        public void CompilationCompleted()
        {
            // nothing to do
        }
    }
}