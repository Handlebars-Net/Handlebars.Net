using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HandlebarsDotNet.Compiler.Translation.Expression.Accessors
{
    internal class EnumerableMemberAccessor : IMemberAccessor
    {
        private static readonly Regex IndexRegex = new Regex(@"^\[?(?<index>\d+)\]?$", RegexOptions.None);

        /// <summary>
        /// Determines if the memberName passed later should be the memberName or the resolvedMemberName.
        /// </summary>
        public bool RequiresResolvedMemberName { get { return false; } }

        /// <summary>
        /// Handles if we try to access an index of 
        /// </summary>
        /// <param name="instance">Instance of the object to access.</param>
        /// <param name="memberName">Member of the instance to access.</param>
        /// <returns></returns>
        public bool CanHandle(object instance, string memberName)
        {
            var canHandle = instance is IEnumerable<object>;
            canHandle = canHandle && IndexRegex.Match(memberName).Success;

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
            object result = null;
            int index;
            
            if (TryGetIndex(memberName, out index))
            {
                var enumerable = (IEnumerable<object>)instance;
                result = enumerable.ElementAtOrDefault(index);
            }

            return result ?? new UndefinedBindingResult();
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
