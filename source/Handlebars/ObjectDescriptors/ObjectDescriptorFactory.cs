using System;
using System.Collections.Generic;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.EqualityComparers;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal class ObjectDescriptorFactory : IObjectDescriptorProvider
    {
        private readonly List<IObjectDescriptorProvider> _providers;
        private readonly LookupSlim<Type, DeferredValue<Type, ObjectDescriptor>, ReferenceEqualityComparer<Type>> _descriptorsCache = new LookupSlim<Type, DeferredValue<Type, ObjectDescriptor>, ReferenceEqualityComparer<Type>>(new ReferenceEqualityComparer<Type>());

        private static readonly Func<Type, List<IObjectDescriptorProvider>, DeferredValue<Type, ObjectDescriptor>> ValueFactory = (key, providers) => new DeferredValue<Type, ObjectDescriptor>(key, t =>
        {
            for (var index = 0; index < providers.Count; index++)
            {
                if (!providers[index].TryGetDescriptor(t, out var descriptor)) continue;

                return descriptor;
            }

            return ObjectDescriptor.Empty;
        });

        public ObjectDescriptorFactory(List<IObjectDescriptorProvider> providers)
        {
            _providers = providers;
        }
        
        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            var deferredValue = _descriptorsCache.GetOrAdd(type, ValueFactory, _providers);
            value = deferredValue.Value;
            return !ReferenceEquals(value, ObjectDescriptor.Empty);
        }
    }
}