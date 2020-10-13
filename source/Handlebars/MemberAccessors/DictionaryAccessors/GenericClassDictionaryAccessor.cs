using System;
using System.Collections.Generic;
using System.ComponentModel;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.MemberAccessors.DictionaryAccessors
{
    internal class GenericClassDictionaryAccessor<T, TK, TV> : IMemberAccessor
        where T: IDictionary<TK, TV>
        where TV: class
    {
        private static readonly TypeConverter TypeConverter = TypeDescriptor.GetConverter(typeof(TK));

        public bool TryGetValue(object instance, ChainSegment memberName, out object value)
        {
            var key = (TK) TypeConverter.ConvertFromString(memberName.TrimmedValue);
            var dictionary = (T) instance;
            if (key != null && dictionary.TryGetValue(key, out var v))
            {
                value = v;
                return true;
            }

            value = null;
            return false;
        }
    }
}