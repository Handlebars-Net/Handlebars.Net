using System;
using System.Collections.Generic;
using System.Linq;

namespace HandlebarsDotNet.Compiler
{
    /// <summary/>
    /// <param name="parameters">Parameters passed to BlockParams.</param>
    /// <param name="valueBinder">Function that perform binding of parameter to <see cref="BindingContext"/>.</param>
    public delegate void ConfigureBlockParams(string[] parameters, ValueBinder valueBinder);
        
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

        public BlockParamsValueProvider(BindingContext context, BlockParam @params)
        {
            _params = @params;
            _invoker = action => action(context);
            _accessors = new Dictionary<string, Func<object>>(StringComparer.OrdinalIgnoreCase);
            
            context.RegisterValueProvider(this);
        }

        public ValueTypes SupportedValueTypes { get; } = ValueTypes.Context | ValueTypes.All;

        /// <summary>
        /// Configures behavior of BlockParams.
        /// </summary>
        public void Configure(ConfigureBlockParams blockParamsConfiguration)
        {
            if(_params == null) return;
            
            void BlockParamsAction(BindingContext context)
            {
                void ValueBinder(string name, Func<object> value)
                {
                    if (!string.IsNullOrEmpty(name)) _accessors[name] = value;
                }
                
                blockParamsConfiguration.Invoke(_params.Parameters, ValueBinder);
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