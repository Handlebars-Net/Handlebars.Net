using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HandlebarsDotNet.MemberAccessors;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal class DictionaryObjectDescriptor : IObjectDescriptorProvider
    {
        public bool CanHandleType(Type type)
        {
            return typeof(IDictionary).IsAssignableFrom(type);
        }

        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            value = new ObjectDescriptor(type)
            {
                GetProperties = GetProperties,
                MemberAccessor = new DictionaryMemberAccessor()
            };

            return true;
        }

        private static IEnumerable<object> GetProperties(object arg)
        {
            var dictionary = (IDictionary) arg;
            foreach (var key in dictionary.Keys)
            {
                yield return key;
            }
        }
    }
}