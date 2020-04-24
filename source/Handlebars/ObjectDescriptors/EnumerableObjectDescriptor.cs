using System;
using System.Collections;
using System.Reflection;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal class EnumerableObjectDescriptor : IObjectDescriptorProvider
    {
        private readonly CollectionObjectDescriptor _collectionObjectDescriptor;

        public EnumerableObjectDescriptor(CollectionObjectDescriptor collectionObjectDescriptor)
        {
            _collectionObjectDescriptor = collectionObjectDescriptor;
        }

        public bool CanHandleType(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
        }

        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            return _collectionObjectDescriptor.TryGetDescriptor(type, out value);
        }
    }
}