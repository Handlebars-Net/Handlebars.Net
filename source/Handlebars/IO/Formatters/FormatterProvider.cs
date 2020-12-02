using System;
using System.Collections.Generic;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.EqualityComparers;
using HandlebarsDotNet.Runtime;
using LookupSlim = HandlebarsDotNet.Collections.LookupSlim<System.Type, HandlebarsDotNet.Runtime.DeferredValue<System.Collections.Generic.KeyValuePair<System.Type, HandlebarsDotNet.Collections.ObservableList<HandlebarsDotNet.IO.IFormatterProvider>>, HandlebarsDotNet.IO.IFormatter>, HandlebarsDotNet.EqualityComparers.ReferenceEqualityComparer<System.Type>>;

namespace HandlebarsDotNet.IO
{
    public class FormatterProvider : IFormatterProvider
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

        public static FormatterProvider Current => AmbientContext.Current?.FormatterProvider;
        
        private readonly LookupSlim _formatters = new LookupSlim(new ReferenceEqualityComparer<Type>());

        private readonly ObservableList<IFormatterProvider> _formatterProviders;

        public FormatterProvider(ObservableList<IFormatterProvider> providers = null)
        {
            _formatterProviders = new ObservableList<IFormatterProvider>();
            
            if (providers != null) Append(providers);
            
            var observer = new ObserverBuilder<ObservableEvent<IFormatterProvider>>()
                .OnEvent<AddedObservableEvent<IFormatterProvider>, LookupSlim>(
                    _formatters, (@event, state) => state.Clear()
                )
                .Build();

            _formatterProviders.Subscribe(observer);
        }

        public FormatterProvider Append(ObservableList<IFormatterProvider> providers)
        {
            _formatterProviders.AddMany(providers);
            providers.Subscribe(_formatterProviders);

            return this;
        }
        
        public FormatterProvider Append(FormatterProvider provider)
        {
            _formatterProviders.AddMany(provider._formatterProviders);
            provider._formatterProviders.Subscribe(_formatterProviders);
            
            return this;
        }
        
        public bool TryCreateFormatter(Type type, out IFormatter formatter)
        {
            formatter = _formatters.GetOrAdd(type, ValueFactory, _formatterProviders).Value;
            return formatter != null;
        }
    }
}