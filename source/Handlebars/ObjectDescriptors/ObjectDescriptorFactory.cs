using System;
using System.Collections.Generic;
using HandlebarsDotNet.Collections;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal class ObjectDescriptorFactory : IObjectDescriptorProvider
    {
        private readonly IList<IObjectDescriptorProvider> _providers;
        private readonly RefLookup<Type, DeferredValue<ObjectDescriptor>> _descriptors = new RefLookup<Type, DeferredValue<ObjectDescriptor>>();
        private readonly RefLookup<Type, DeferredValue<bool>> _supportedTypes = new RefLookup<Type, DeferredValue<bool>>();

        public ObjectDescriptorFactory(IList<IObjectDescriptorProvider> providers)
        {
            _providers = providers;
        }

        public bool CanHandleType(Type type)
        {
            if (_supportedTypes.ContainsKey(type))
            {
                ref var contains = ref _supportedTypes.GetValueOrDefault(type);
                return contains.Value;
            }
            
            ref var deferredValue = ref _supportedTypes.GetOrAdd(type, SupportedTypesValueFactory);
            return deferredValue.Value;
        }

        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            value = null;
            ObjectDescriptor descriptor;
            if (_descriptors.ContainsKey(type))
            {
                ref var existingDeferredValue = ref _descriptors.GetValueOrDefault(type);
                descriptor = existingDeferredValue.Value;
            }
            else
            {
                ref var deferredValue = ref _descriptors.GetOrAdd(type, DescriptorsValueFactory);
                descriptor = deferredValue.Value;    
            }
            
            if (descriptor == null) return false;

            value = descriptor;
            return true;
        }
        
        private ref DeferredValue<bool> SupportedTypesValueFactory(Type type, ref DeferredValue<bool> deferredValue)
        {
            deferredValue.Factory = () =>
            {
                for (var index = 0; index < _providers.Count; index++)
                {
                    if (_providers[index].CanHandleType(type)) return true;
                }

                return false;
            };
            return ref deferredValue;
        }

        private ref DeferredValue<ObjectDescriptor> DescriptorsValueFactory(Type type, ref DeferredValue<ObjectDescriptor> deferredValue)
        {
            deferredValue.Factory = () =>
            {
                for (var index = 0; index < _providers.Count; index++)
                {
                    var provider = _providers[index];
                    if (!provider.CanHandleType(type)) continue;
                    if (provider.TryGetDescriptor(type, out var value))
                    {
                        return value;
                    }
                }

                return null;
            };

            return ref deferredValue;
        }
    }
}