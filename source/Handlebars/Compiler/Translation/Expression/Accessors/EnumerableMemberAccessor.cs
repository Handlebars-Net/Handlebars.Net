using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HandlebarsDotNet.Compiler.Translation.Expression.Accessors
{
    public class EnumerableMemberAccessor : IMemberAccessor
    {
        private static readonly Regex IndexRegex = new Regex(@"^\[?(?<index>\d+)\]?$", RegexOptions.None);

        /// <summary>
        /// Determines if the memberName passed later should be the memberName or the resolvedMemberName.
        /// </summary>
        public bool RequiresResolvedMemberName { get { return true; } }

        /// <summary>
        /// Determines if a member can be accessed using the current accessor.
        /// </summary>
        /// <param name="instance">Instance of the object to access.</param>
        /// <returns></returns>
        public bool CanHandle(object instance)
        {
            //To check if this works with all enumerators.
            var canHandle = instance is IEnumerable<object>;
            return canHandle;
        }

        /// <summary>
        /// Accesses the member and retrieves the result.
        /// </summary>
        /// <param name="instance">Instance of the object to access.</param>
        /// <param name="memberName">Member of the instance to access.</param>
        /// <returns></returns>
        public object AccessMember(object instance, string memberName)
        {
            int index;
            //object result = null;
            var enumerable = (IEnumerable<object>) instance;
            var match = IndexRegex.Match(memberName);

            if (match.Success)
            {
                if (match.Groups["index"].Success == false || int.TryParse(match.Groups["index"].Value, out index) == false)
                {
                    return new UndefinedBindingResult();
                }

                var result = enumerable.ElementAtOrDefault(index);

                return result ?? new UndefinedBindingResult();
            }

            return new UndefinedBindingResult();
        }

        private bool TryGetIndex(string memberName, out int index)
        {
            var match = IndexRegex.Match(memberName);

            if (match.Success)
            {
                if (match.Groups["index"].Success)
                {
                    var indexGroup = match.Groups["index"].Value;
                    var success = int.TryParse(indexGroup, out index);
                    return success;
                }
            }

            index = 0;
            return false;
        }
    }
}
