using System;
using System.Collections.Generic;
using System.Linq;

namespace HandlebarsDotNet.Compiler
{
    /// <summary/>
    /// <param name="parameters">Parameters passed to BlockParams.</param>
    /// <param name="valueBinder">Function that perform binding of parameter to <see cref="BindingContext"/>.</param>
    public delegate void ConfigureBlockParams(IReadOnlyDictionary<string, Lazy<object>> parameters, ValueBinder valueBinder);
        
    /// <summary>
    /// Function that perform binding of parameter to <see cref="BindingContext"/>.
    /// </summary>
    /// <param name="variableName">Variable name that would be added to the <see cref="BindingContext"/>.</param>
    /// <param name="valueProvider">Variable value provider that would be invoked when <paramref name="variableName"/> is requested.</param>
    public delegate void ValueBinder(string variableName, Func<object> valueProvider);
    
    /// <summary/>
    internal class BlockParamsValueProvider : IValueProvider
    {
        private readonly BlockParam _params;
        private readonly Action<Action<BindingContext>> _invoker;
        private readonly Dictionary<string, Func<object>> _accessors;
        private readonly PathResolver _pathResolver;
        
        public BlockParamsValueProvider(BindingContext context, HandlebarsConfiguration configuration, object @params)
        {
            _params = @params as BlockParam;
            _invoker = action => action(context);
            _pathResolver = new PathResolver(configuration);
            _accessors = new Dictionary<string, Func<object>>(StringComparer.OrdinalIgnoreCase);
            
            context.RegisterValueProvider(this);
        }

        public bool ProvidesNonContextVariables { get; } = true;

        /// <summary>
        /// Configures behavior of BlockParams.
        /// </summary>
        public void Configure(ConfigureBlockParams blockParamsConfiguration)
        {
            if(_params == null) return;

            Lazy<object> ValueAccessor(BindingContext context, string paramName) => 
                new Lazy<object>(() => _pathResolver.ResolvePath(context, paramName));
            
            void BlockParamsAction(BindingContext context)
            {
                void ValueBinder(string name, Func<object> value)
                {
                    if (!string.IsNullOrEmpty(name)) _accessors[name] = value;
                }

                var values = _params.Parameters
                    .ToDictionary(parameter => parameter, parameter => ValueAccessor(context, parameter));
                
                blockParamsConfiguration.Invoke(values, ValueBinder);
            }

            _invoker(BlockParamsAction);
        }

        public bool TryGetValue(string param, out object value)
        {
            if (_accessors.TryGetValue(param, out var valueProvider))
            {
                value = valueProvider();
                return true;
            }

            value = null;
            return false;
        }
    }
}