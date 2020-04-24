using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

namespace HandlebarsDotNet.MemberAccessors
{
    internal class EnumerableMemberAccessor : IMemberAccessor
    {
        private static readonly Regex IndexRegex = new Regex(@"^\[?(?<index>\d+)\]?$", RegexOptions.Compiled);

        public bool TryGetValue(object instance, Type type, string memberName, out object value)
        {
            value = null;

            var match = IndexRegex.Match(memberName);
            if (!match.Success) return false;
            const string indexGroupName = "index";
            if (!match.Groups[indexGroupName].Success || !int.TryParse(match.Groups[indexGroupName].Value, out var index)) return false;

            switch (instance)
            {
                case IList list: 
                    value = list[index];
                    return true;
                    
                case IEnumerable enumerable: 
                    value = enumerable.Cast<object>().ElementAtOrDefault(index);
                    return true;
            }
                
            return false;
        }
    }
}