using System.Collections;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.MemberAccessors
{
    public sealed class DictionaryMemberAccessor : IMemberAccessor
    {
        public bool TryGetValue(object instance, ChainSegment memberName, out object? value)
        {
            value = null;
            // Check if the instance is IDictionary (ie, System.Collections.Hashtable)
            // Only string keys supported - indexer takes an object, but no nice
            // way to check if the hashtable check if it should be a different type.
            var dictionary = (IDictionary) instance;

            // Try the original-case key first to support camelCase and PascalCase keys
            if (dictionary.Contains(memberName.TrimmedValue))
            {
                value = dictionary[memberName.TrimmedValue];
                return true;
            }

            // Fall back to lowercase key for backward compatibility
            if (memberName.LowerInvariant != memberName.TrimmedValue && dictionary.Contains(memberName.LowerInvariant))
            {
                value = dictionary[memberName.LowerInvariant];
                return true;
            }

            return false;
        }
    }
}