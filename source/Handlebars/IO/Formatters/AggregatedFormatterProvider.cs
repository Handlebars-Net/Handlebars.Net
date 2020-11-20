using System;
using System.Collections.Generic;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.EqualityComparers;
using HandlebarsDotNet.Runtime;
using LookupSlim = HandlebarsDotNet.Collections.LookupSlim<System.Type, HandlebarsDotNet.Runtime.DeferredValue<System.Collections.Generic.KeyValuePair<System.Type, HandlebarsDotNet.Collections.ObservableList<HandlebarsDotNet.IO.IFormatterProvider>>, HandlebarsDotNet.IO.IFormatter>, HandlebarsDotNet.EqualityComparers.ReferenceEqualityComparer<System.Type>>;

namespace HandlebarsDotNet.IO
{
    internal class AggregatedFormatterProvider : IFormatterProvider
    {
        private static readonly Func<Type, ObservableList<IFormatterProvider>, DeferredValue<KeyValuePair<Type, ObservableList<IFormatterProvider>>, IFormatter>> ValueFactory = (t, providers) =>
        {
            return new DeferredValue<KeyValuePair<Type, ObservableList<IFormatterProvider>>, IFormatter>(
                new KeyValuePair<Type, ObservableList<IFormatterProvider>>(t, providers), 
                DeferredValueFactory
            );
        };

        private static readonly Func<KeyValuePair<Type, ObservableList<IFormatterProvider>>, IFormatter> DeferredValueFactory = deps =>
        {
            var formatterProviders = deps.Value;
            for (var index = formatterProviders.Count - 1; index >= 0; index--)
            {
                if (!formatterProviders[index].TryCreateFormatter(deps.Key, out var value)) continue;
                return value;
            }

            return null;
        };
        
        private readonly LookupSlim _formatters = new LookupSlim(new ReferenceEqualityComparer<Type>());

        private readonly ObservableList<IFormatterProvider> _formatterProviders;

        public AggregatedFormatterProvider(ObservableList<IFormatterProvider> formatterProviders)
        {
            _formatterProviders = formatterProviders;
            
            var observer = new ObserverBuilder<ObservableEvent<IFormatterProvider>>()
                .OnEvent<AddedObservableEvent<IFormatterProvider>, LookupSlim>(
                    _formatters, (@event, state) => state.Clear()
                )
                .Build();
                
            formatterProviders.Subscribe(observer);
        }
        
        public bool TryCreateFormatter(Type type, out IFormatter formatter)
        {
            formatter = _formatters.GetOrAdd(type, ValueFactory, _formatterProviders).Value;
            return formatter != null;
        }
    }
}