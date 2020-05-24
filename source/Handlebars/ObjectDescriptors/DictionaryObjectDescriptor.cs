using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HandlebarsDotNet.MemberAccessors;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal class DictionaryObjectDescriptor : IObjectDescriptorProvider
    {
        private static readonly DictionaryMemberAccessor DictionaryMemberAccessor = new DictionaryMemberAccessor();

        private static readonly Func<ObjectDescriptor, object, IEnumerable<object>> GetProperties = (descriptor, arg) =>
        {
            return Enumerate((IDictionary) arg);
            
            IEnumerable<object> Enumerate(IDictionary dictionary)
            {
                foreach (var key in dictionary.Keys) yield return key;
            }
        };

        public bool CanHandleType(Type type)
        {
            return typeof(IDictionary).IsAssignableFrom(type);
        }

        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            value = new ObjectDescriptor(type, DictionaryMemberAccessor, GetProperties);

            return true;
        }
    }
}