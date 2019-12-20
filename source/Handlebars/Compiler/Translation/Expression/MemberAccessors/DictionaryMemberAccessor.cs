using System.Collections;

namespace HandlebarsDotNet.Compiler
{
    internal class DictionaryMemberAccessor : MemberAccessor
    {
        public DictionaryMemberAccessor(CompilationContext compilationContext) : base(compilationContext)
        {
        }

        public override bool TryAccessMember(BindingContext context, object instance, string memberName, out object result)
        {
            result = null;

            // Check if the instance is IDictionary (ie, System.Collections.Hashtable)
            if (!(instance is IDictionary dictionary)) return false;

            memberName = ResolveMemberName(instance, memberName);
            object key = ContainsVariable(memberName)
                ? context.GetContextVariable(memberName.Substring(1))
                : memberName;

            // Only string keys supported - indexer takes an object, but no nice
            // way to check if the hashtable check if it should be a different type.
            if (!dictionary.Contains(key)) return false;

            result = dictionary[key];
            return true;
        }
    }
}