using System;
using System.Collections.Generic;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.ValueProviders;
using Microsoft.Extensions.ObjectPool;

namespace HandlebarsDotNet.Compiler
{
    /*
     * Is going to be changed in next iterations
     */
    
    /// <summary>
    /// Configures BlockParameters for current BlockHelper
    /// </summary>
    /// <param name="parameters">Parameters passed to BlockParams.</param>
    /// <param name="valueBinder">Function that perform binding of parameter to <see cref="BindingContext"/>.</param>
    /// <param name="dependencies">Dependencies of current configuration. Used to omit closure creation.</param>
    public delegate void ConfigureBlockParams(string[] parameters, ValueBinder valueBinder, object[] dependencies);
        
    /// <summary>
    /// Function that perform binding of parameter to <see cref="BindingContext"/>.
    /// </summary>
    /// <param name="variableName">Variable name that would be added to the <see cref="BindingContext"/>.</param>
    /// <param name="valueProvider">Variable value provider that would be invoked when <paramref name="variableName"/> is requested.</param>
    /// <param name="context">Context for the binding.</param>
    public delegate void ValueBinder(string variableName, Func<object, object> valueProvider, object context = null);
    
    /// <summary/>
    internal class BlockParamsValueProvider : IValueProvider
    {
        private static readonly string[] EmptyParameters = new string[0];

        private static readonly BlockParamsValueProviderPool Pool = new BlockParamsValueProviderPool();
        
        private readonly Dictionary<string, KeyValuePair<object, Func<object, object>>> _accessors;
        
        private BlockParam _params;
        private Action<Action<BindingContext>> _invoker;

        public static BlockParamsValueProvider Create(BindingContext context, object @params)
        {
            var blockParamsValueProvider = Pool.Get();

            blockParamsValueProvider._params = @params as BlockParam;
            blockParamsValueProvider._invoker = action => action(context);

            return blockParamsValueProvider;
        }

        private BlockParamsValueProvider()
        {
            _accessors = new Dictionary<string, KeyValuePair<object, Func<object, object>>>(StringComparer.OrdinalIgnoreCase);
        }

        public ValueTypes SupportedValueTypes { get; } = ValueTypes.Context | ValueTypes.All;

        /// <summary>
        /// Configures behavior of BlockParams.
        /// </summary>
        public void Configure(ConfigureBlockParams blockParamsConfiguration, params object[] dependencies)
        {
            var parameters = _params?.Parameters ?? EmptyParameters;
            void BlockParamsAction(BindingContext context)
            {
                void ValueBinder(string name, Func<object, object> value, object ctx)
                {
                    if (!string.IsNullOrEmpty(name)) _accessors[name] = new KeyValuePair<object, Func<object, object>>(ctx, value);
                }
                
                blockParamsConfiguration.Invoke(parameters, ValueBinder, dependencies);
            }

            _invoker(BlockParamsAction);
        }

        public bool TryGetValue(ref ChainSegment segment, out object value)
        {
            if (_accessors.TryGetValue(segment.LowerInvariant, out var provider))
            {
                value = provider.Value(provider.Key);
                return true;
            }

            value = null;
            return false;
        }

        public void Dispose()
        {
            Pool.Return(this);
        }

        private class BlockParamsValueProviderPool : DefaultObjectPool<BlockParamsValueProvider>
        {
            public BlockParamsValueProviderPool() : base(new BlockParamsValueProviderPolicy())
            {
            }
            
            private class BlockParamsValueProviderPolicy : IPooledObjectPolicy<BlockParamsValueProvider>
            {
                public BlockParamsValueProvider Create()
                {
                    return new BlockParamsValueProvider();
                }

                public bool Return(BlockParamsValueProvider item)
                {
                    item._accessors.Clear();
                    item._invoker = null;
                    item._params = null;

                    return true;
                }
            }
        }
    }
}