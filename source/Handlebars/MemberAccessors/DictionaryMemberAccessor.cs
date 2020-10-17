using System;
using System.Collections;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.MemberAccessors
{
    internal class DictionaryMemberAccessor : IMemberAccessor
    {
        public bool TryGetValue(object instance, ChainSegment memberName, out object value)
        {
            value = null;
            // Check if the instance is IDictionary (ie, System.Collections.Hashtable)
            // Only string keys supported - indexer takes an object, but no nice
            // way to check if the hashtable check if it should be a different type.
            var dictionary = (IDictionary) instance;
            value = dictionary[memberName.LowerInvariant];
            return true;
        }
    }
}