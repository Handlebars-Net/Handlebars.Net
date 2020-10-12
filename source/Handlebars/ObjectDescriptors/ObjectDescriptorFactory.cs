using System;
using System.Collections.Generic;
using HandlebarsDotNet.Collections;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal class ObjectDescriptorFactory : IObjectDescriptorProvider
    {
        private readonly IList<IObjectDescriptorProvider> _providers;
        private readonly HashSetSlim<Type> _descriptorsNegativeCache = new HashSetSlim<Type>();
        private readonly LookupSlim<Type, DeferredValue<Type, ObjectDescriptor>> _descriptorsCache = new LookupSlim<Type, DeferredValue<Type, ObjectDescriptor>>();

        private static readonly Func<Type, IList<IObjectDescriptorProvider>, DeferredValue<Type, ObjectDescriptor>> ValueFactory = (key, providers) => new DeferredValue<Type, ObjectDescriptor>(key, t =>
        {
            for (var index = 0; index < providers.Count; index++)
            {
                var descriptorProvider = providers[index];
                if (!descriptorProvider.CanHandleType(t)) continue;
                if (!descriptorProvider.TryGetDescriptor(t, out var descriptor)) continue;

                return descriptor;
            }

            return ObjectDescriptor.Empty;
        });

        public ObjectDescriptorFactory(IList<IObjectDescriptorProvider> providers)
        {
            _providers = providers;
        }

        public bool CanHandleType(Type type)
        {
            if (_descriptorsNegativeCache.Contains(type)) return false;
            if (_descriptorsCache.TryGetValue(type, out var deferredValue) && !ReferenceEquals(deferredValue.Value, ObjectDescriptor.Empty)) return true;

            deferredValue = _descriptorsCache.GetOrAdd(type, ValueFactory, _providers);
            return !ReferenceEquals(deferredValue.Value, ObjectDescriptor.Empty);
        }

        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            if (_descriptorsCache.TryGetValue(type, out var deferredValue))
            {
                value = deferredValue.Value;
                return true;
            }
            
            value = ObjectDescriptor.Empty;
            return false;
        }
    }
}