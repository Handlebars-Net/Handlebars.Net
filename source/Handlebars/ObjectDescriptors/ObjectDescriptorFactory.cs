using System;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.EqualityComparers;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet.ObjectDescriptors
{
    public class ObjectDescriptorFactory : IObjectDescriptorProvider, IObserver<ObservableEvent<IObjectDescriptorProvider>>
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

        private readonly IObserver<ObservableEvent<IObjectDescriptorProvider>> _observer;

        public static ObjectDescriptorFactory Current => AmbientContext.Current?.ObjectDescriptorFactory;
        
        public ObjectDescriptorFactory(ObservableList<IObjectDescriptorProvider> providers = null)
        {
            _providers = new ObservableList<IObjectDescriptorProvider>();
             
            if (providers != null) Append(providers);
            
            _observer = ObserverBuilder<ObservableEvent<IObjectDescriptorProvider>>.Create(_descriptorsCache)
                .OnEvent<AddedObservableEvent<IObjectDescriptorProvider>>((@event, state) => state.Clear())
                .Build();
            
            _providers.Subscribe(this);
        }

        public ObjectDescriptorFactory Append(ObservableList<IObjectDescriptorProvider> providers)
        {
            _providers.AddMany(providers);
            providers.Subscribe(_providers);

            return this;
        }
        
        public ObjectDescriptorFactory Append(ObjectDescriptorFactory factory)
        {
            _providers.AddMany(factory._providers);
            factory._providers.Subscribe(_providers);
            
            return this;
        }
        
        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            value = _descriptorsCache.GetOrAdd(type, ValueFactory, _providers).Value;
            return !ReferenceEquals(value, ObjectDescriptor.Empty);
        }

        public void OnCompleted() => _observer.OnCompleted();

        public void OnError(Exception error) => _observer.OnError(error);

        public void OnNext(ObservableEvent<IObjectDescriptorProvider> value) => _observer.OnNext(value);
    }
}