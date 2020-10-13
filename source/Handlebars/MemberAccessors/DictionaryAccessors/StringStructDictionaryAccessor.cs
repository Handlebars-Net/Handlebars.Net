using System;
using System.Collections.Generic;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.MemberAccessors.DictionaryAccessors
{
    internal sealed class StringStructDictionaryAccessor<T, TV> : IMemberAccessor
        where T: IDictionary<string, TV>
        where TV: struct
    {
        private static readonly object BoxedDefault = default(TV);
        
        public bool TryGetValue(object instance, ChainSegment memberName, out object value)
        {
            var dictionary = (T) instance;
            if (dictionary.TryGetValue(memberName.TrimmedValue, out var v))
            {
                value = v;
                return true;
            }

            value = BoxedDefault;
            return false;
        }
    }
}