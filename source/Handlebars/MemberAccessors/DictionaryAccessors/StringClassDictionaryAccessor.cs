using System;
using System.Collections.Generic;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.MemberAccessors.DictionaryAccessors
{
    internal sealed class StringClassDictionaryAccessor<T, TV> : IMemberAccessor
        where T: IDictionary<string, TV>
        where TV: class
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