using System;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.EqualityComparers;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal class ObjectDescriptorFactory : IObjectDescriptorProvider
    {
        private readonly ObservableList<IObjectDescriptorProvider> _providers;
        private readonly LookupSlim<Type, DeferredValue<Type, ObjectDescriptor>, ReferenceEqualityComparer<Type>> _descriptorsCache = new LookupSlim<Type, DeferredValue<Type, ObjectDescriptor>, ReferenceEqualityComparer<Type>>(new ReferenceEqualityComparer<Type>());

        private static readonly Func<Type, ObservableList<IObjectDescriptorProvider>, DeferredValue<Type, ObjectDescriptor>> ValueFactory = (key, providers) => new DeferredValue<Type, ObjectDescriptor>(key, t =>
        {
            for (var index = providers.Count - 1; index >= 0; index--)
            {
                if (!providers[index].TryGetDescriptor(t, out var descriptor)) continue;

                return descriptor;
            }

            return ObjectDescriptor.Empty;
        });

        public ObjectDescriptorFactory(ObservableList<IObjectDescriptorProvider> providers)
        {
            _providers = providers;

            var observer = new ObserverBuilder<ObservableEvent<IObjectDescriptorProvider>>()
                .OnEvent<
                    AddedObservableEvent<IObjectDescriptorProvider>,
                    LookupSlim<Type, DeferredValue<Type, ObjectDescriptor>, ReferenceEqualityComparer<Type>>
                >(_descriptorsCache, (@event, state) => state.Clear()).Build();
            
            providers.Subscribe(observer);
        }
        
        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            value = _descriptorsCache.GetOrAdd(type, ValueFactory, _providers).Value;
            return !ReferenceEquals(value, ObjectDescriptor.Empty);
        }
    }
}