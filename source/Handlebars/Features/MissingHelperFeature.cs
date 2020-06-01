using System;
using System.Linq;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.Helpers;

namespace HandlebarsDotNet.Features
{
    /// <summary>
    /// 
    /// </summary>
    public static class MissingHelperFeatureExtension
    {
        /// <summary>
        /// Allows to intercept calls to missing helpers.
        /// <para>For Handlebarsjs docs see: https://handlebarsjs.com/guide/hooks.html#helpermissing</para>
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="helperMissing">Delegate that returns interceptor for <see cref="HandlebarsReturnHelper"/> and <see cref="HandlebarsHelper"/></param>
        /// <param name="log">Delegate would be invoked before returning <paramref name="helperMissing"/> interceptors</param>
        /// <returns></returns>
        public static HandlebarsConfiguration RegisterMissingHelperHook(
            this HandlebarsConfiguration configuration, 
            HandlebarsReturnHelper helperMissing = null,
            Action<object, object[]> log = null
        )
        {
            HandlebarsReturnHelper logger = null;
            if (log != null)
            {
                logger = (context, arguments) =>
                {
                    log(context, arguments);
                    return string.Empty;
                };
            }
            
            var feature = new MissingHelperFeatureFactory(helperMissing, logger);
            configuration.CompileTimeConfiguration.Features.Add(feature);

            return configuration;
        }
    }
    
    internal class MissingHelperFeatureFactory : IFeatureFactory
    {
        private readonly HandlebarsReturnHelper _returnHelper;
        private readonly HandlebarsReturnHelper _log;

        public MissingHelperFeatureFactory(
            HandlebarsReturnHelper returnHelper = null,
            HandlebarsReturnHelper log = null
        )
        {
            _returnHelper = returnHelper;
            _log = log;
        }

        public IFeature CreateFeature()
        {
            return new MissingHelperFeature(_returnHelper, _log);
        }
    }
    
    internal class MissingHelperFeature : IFeature, IHelperResolver
    {
        private ICompiledHandlebarsConfiguration _configuration;
        private HandlebarsReturnHelper _returnHelper;
        private HandlebarsBlockHelper _blockHelper;
        private HandlebarsReturnHelper _log;

        public MissingHelperFeature(
            HandlebarsReturnHelper returnHelper,
            HandlebarsReturnHelper log
        )
        {
            _returnHelper = returnHelper;
            _log = log;
            
            if (_returnHelper != null)
            {
                _blockHelper = (output, options, context, arguments) =>
                {
                    var result = returnHelper((object) context, arguments);
                    output.WriteSafeString(result);
                };
            }
        }

        public void OnCompiling(ICompiledHandlebarsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void CompilationCompleted()
        {
            var existingRegistrations = _configuration.HelperResolvers
                .OfType<MissingHelperFeature>()
                .ToList();

            if (existingRegistrations.Any())
            {
                existingRegistrations.ForEach(o => _configuration.HelperResolvers.Remove(o));
            }
            
            if (_configuration.ReturnHelpers.TryGetValue("helperMissing", out var returnHelperMissing))
            {
                if(_returnHelper == null) _returnHelper = returnHelperMissing;
                if (_blockHelper == null)
                {
                    _blockHelper = (output, options, context, arguments) =>
                    {
                        var result = returnHelperMissing((object) context, arguments);
                        output.WriteSafeString(result);
                    };
                }
            }

            if (_log == null && _configuration.ReturnHelpers.TryGetValue("log", out var logger))
            {
                _log = logger;
            }

            if (_log != null)
            {
                var currentReturnHelper = _returnHelper;
                _returnHelper = (context, arguments) =>
                {
                    _log(context, arguments);
                    return currentReturnHelper?.Invoke(context, arguments) ?? string.Empty;
                };

                var currentBlockHelper = _blockHelper;
                _blockHelper = (output, options, context, arguments) =>
                {
                    _log(context, arguments);
                    currentBlockHelper?.Invoke(output, options, context, arguments);
                };
            }

            _configuration.HelperResolvers.Add(this);
        }
        
        public bool TryResolveReturnHelper(string name, Type targetType, out HandlebarsReturnHelper helper)
        {
            if (_returnHelper == null)
            {
                helper = null;
                return false;
            }
            
            helper = (context, arguments) =>
            {
                var instance = (object) context;
                var chainSegment = new ChainSegment(name);
                if (!PathResolver.TryAccessMember(instance, ref chainSegment, _configuration, out var value))
                {
                    var newArguments = new object[arguments.Length + 1];
                    Array.Copy(arguments, newArguments, arguments.Length);
                    newArguments[arguments.Length] = name;
                    
                    return _returnHelper(context, newArguments);
                }
                
                return value;
            };

            return true;
        }

        public bool TryResolveBlockHelper(string name, out HandlebarsBlockHelper helper)
        {
            if (_blockHelper == null)
            {
                helper = null;
                return false;
            }
            
            helper = (output, options, context, arguments) =>
            {
                var newArguments = new object[arguments.Length + 1];
                Array.Copy(arguments, newArguments, arguments.Length);
                newArguments[arguments.Length] = name;

                _blockHelper(output, options, context, newArguments);
            };
            
            return true;
        }
    }
}