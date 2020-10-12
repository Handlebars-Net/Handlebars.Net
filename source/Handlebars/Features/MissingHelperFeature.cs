using System;
using System.Linq;
using HandlebarsDotNet.Adapters;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.Helpers;

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
            HandlebarsReturnHelper helperMissing = null,
            HandlebarsBlockHelper blockHelperMissing = null
        )
        {
            var feature = new MissingHelperFeatureFactory(helperMissing, blockHelperMissing);
            configuration.CompileTimeConfiguration.Features.Add(feature);

            return configuration;
        }
    }

    internal class MissingHelperFeatureFactory : IFeatureFactory
    {
        private readonly HandlebarsReturnHelper _returnHelper;
        private readonly HandlebarsBlockHelper _blockHelper;

        public MissingHelperFeatureFactory(
            HandlebarsReturnHelper returnHelper = null,
            HandlebarsBlockHelper blockHelper = null
        )
        {
            _returnHelper = returnHelper;
            _blockHelper = blockHelper;
        }

        public IFeature CreateFeature() => new MissingHelperFeature(_returnHelper, _blockHelper);
    }

    [FeatureOrder(int.MaxValue)]
    internal class MissingHelperFeature : IFeature, IHelperResolver
    {
        private ICompiledHandlebarsConfiguration _configuration;
        private HandlebarsReturnHelper _returnHelper;
        private HandlebarsBlockHelper _blockHelper;

        public MissingHelperFeature(
            HandlebarsReturnHelper returnHelper,
            HandlebarsBlockHelper blockHelper
        )
        {
            _returnHelper = returnHelper;
            _blockHelper = blockHelper;
        }

        public void OnCompiling(ICompiledHandlebarsConfiguration configuration) => _configuration = configuration;

        public void CompilationCompleted()
        {
            var existingFeatureRegistrations = _configuration
                .HelperResolvers
                .OfType<MissingHelperFeature>()
                .ToList();

            if (existingFeatureRegistrations.Any())
            {
                existingFeatureRegistrations.ForEach(o => _configuration.HelperResolvers.Remove(o));
            }

            ResolveHelpersRegistrations();

            _configuration.HelperResolvers.Add(this);
        }

        public bool TryResolveReturnHelper(string name, Type targetType, out HandlebarsReturnHelper helper)
        {
            if (_returnHelper == null)
            {
                _configuration.ReturnHelpers.TryGetValue("helperMissing", out _returnHelper);
            }

            if (_returnHelper == null && _configuration.Helpers.TryGetValue("helperMissing", out var simpleHelper))
            {
                _returnHelper = new HelperToReturnHelperAdapter(simpleHelper);
            }

            if (_returnHelper == null)
            {
                helper = null;
                return false;
            }

            helper = (context, arguments) =>
            {
                var instance = (object) context;
                var chainSegment = new ChainSegment(name);
                if (PathResolver.TryAccessMember(instance, ref chainSegment, _configuration, out var value))
                    return value;

                var newArguments = new object[arguments.Length + 1];
                Array.Copy(arguments, newArguments, arguments.Length);
                newArguments[arguments.Length] = name;

                return _returnHelper(context, newArguments);
            };

            return true;
        }

        public bool TryResolveBlockHelper(string name, out HandlebarsBlockHelper helper)
        {
            if (_blockHelper == null)
            {
                _configuration.BlockHelpers.TryGetValue("blockHelperMissing", out _blockHelper);
            }

            if (_blockHelper == null)
            {
                helper = null;
                return false;
            }

            helper = (output, options, context, arguments) =>
            {
                options["name"] = name;
                _blockHelper(output, options, context, arguments);
            };

            return true;
        }

        private void ResolveHelpersRegistrations()
        {
            if (_returnHelper == null && _configuration.Helpers.TryGetValue("helperMissing", out var helperMissing))
            {
                _returnHelper = new HelperToReturnHelperAdapter(helperMissing);
            }

            if (_returnHelper == null &&
                _configuration.ReturnHelpers.TryGetValue("helperMissing", out var returnHelperMissing))
            {
                _returnHelper = returnHelperMissing;
            }

            if (_blockHelper == null &&
                _configuration.BlockHelpers.TryGetValue("blockHelperMissing", out var blockHelperMissing))
            {
                _blockHelper = blockHelperMissing;
            }
        }
    }
}