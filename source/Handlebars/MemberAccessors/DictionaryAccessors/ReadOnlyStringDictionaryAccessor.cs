using System.Collections.Generic;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.MemberAccessors.DictionaryAccessors
{
    public sealed class ReadOnlyStringDictionaryAccessor<T, TV> : IMemberAccessor
        where T: IReadOnlyDictionary<string, TV>
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