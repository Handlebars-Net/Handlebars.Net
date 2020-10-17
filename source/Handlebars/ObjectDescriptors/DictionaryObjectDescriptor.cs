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

            static IEnumerable<object> Enumerate(IDictionary dictionary)
            {
                foreach (var key in dictionary.Keys) yield return key;
            }
        };

        private static readonly Type Type = typeof(IDictionary);

        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            if (!Type.IsAssignableFrom(type))
            {
                value = ObjectDescriptor.Empty;;
                return false;
            }
            
            value = new ObjectDescriptor(type, DictionaryMemberAccessor, GetProperties);

            return true;
        }
    }
}