using System.Collections;

namespace HandlebarsDotNet.Compiler.Translation.Expression.Accessors
{
    internal class DictionaryMemberAccessor : IMemberAccessor
    {
        /// <summary>
        /// Determines if the memberName passed later should be the memberName or the resolvedMemberName.
        /// </summary>
        public bool RequiresResolvedMemberName { get { return true; } }

        /// <summary>
        /// Determines if a member can be accessed using the current accessor.
        /// </summary>
        /// <param name="instance">Instance of the object to access.</param>
        /// <param name="memberName">Member of the instance to access.</param>
        /// <returns></returns>
        public bool CanHandle(object instance, string memberName)
        {
            var canHandle = instance is IDictionary;
            return canHandle;
        }

        /// <summary>
        /// Accesses the member and retrieves the result.
        /// </summary>
        /// <param name="instance">Instance of the object to access.</param>
        /// <param name="memberName">Member of the instance to access.</param>
        /// <returns></returns>
        /// <remarks>
        /// Only string keys supported - indexer takes an object, but no nice way to check if the hashtable check if it should be a different type.
        /// </remarks>
        public object AccessMember(object instance, string memberName)
        {
            var key = CleanMemberName(memberName);
            var dictionary = (IDictionary) instance;

            if (dictionary.Contains(key))
            {
                var value = dictionary[key];
                return value;
            }

            return new UndefinedBindingResult();
        }

        private static string CleanMemberName(string memberName)
        {
            return memberName.Trim('[', ']');
        }
    }
}
