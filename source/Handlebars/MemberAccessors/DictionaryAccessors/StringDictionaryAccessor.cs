using System.Collections.Generic;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.MemberAccessors.DictionaryAccessors
{
    public sealed class StringDictionaryAccessor<T, TV> : IMemberAccessor
        where T: IDictionary<string, TV>
    {
        public bool TryGetValue(object instance, ChainSegment memberName, out object value)
        {
            var dictionary = (T) instance;
            if (dictionary.TryGetValue(memberName.TrimmedValue, out var v))
            {
                value = v;
                return true;
            }

            value = null;
            return false;
        }
    }
}