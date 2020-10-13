using System.Linq;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.BlockHelpers;

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
            HelperDescriptorBase helperMissing = null,
            BlockHelperDescriptorBase blockHelperMissing = null
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
        private readonly HelperDescriptorBase _returnHelper;
        private readonly BlockHelperDescriptorBase _blockHelper;

        public MissingHelperFeatureFactory(
            HelperDescriptorBase returnHelper = null,
            BlockHelperDescriptorBase blockHelper = null
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
        
        private readonly HelperDescriptorBase _helper;
        private readonly BlockHelperDescriptorBase _blockHelper;

        public MissingHelperFeature(
            HelperDescriptorBase helper,
            BlockHelperDescriptorBase blockHelper
        )
        {
            _helper = helper;
            _blockHelper = blockHelper;
        }

        public void OnCompiling(ICompiledHandlebarsConfiguration configuration)
        {
            var pathInfoStore = configuration.PathInfoStore;

            var helperMissingPathInfo = pathInfoStore.GetOrAdd(HelperMissingKey);
            if(!configuration.Helpers.ContainsKey(helperMissingPathInfo))
            {
                var helper = _helper ?? new MissingHelperDescriptor();
                configuration.Helpers.AddOrUpdate(helperMissingPathInfo, 
                    h => new StrongBox<HelperDescriptorBase>(h), 
                    (h, o) => o.Value = h, 
                    helper);
            }

            var blockHelperMissingKeyPathInfo = pathInfoStore.GetOrAdd(BlockHelperMissingKey);
            if(!configuration.BlockHelpers.ContainsKey(blockHelperMissingKeyPathInfo))
            {
                var blockHelper = _blockHelper ?? new MissingBlockHelperDescriptor();
                configuration.BlockHelpers.AddOrUpdate(blockHelperMissingKeyPathInfo, 
                    h => new StrongBox<BlockHelperDescriptorBase>(h), 
                    (h, o) => o.Value = h, 
                    blockHelper);
            }
        }

        public void CompilationCompleted()
        {
            // nothing to do
        }
    }
}