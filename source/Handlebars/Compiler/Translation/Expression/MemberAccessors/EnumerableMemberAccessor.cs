using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HandlebarsDotNet.Compiler
{
    internal class EnumerableMemberAccessor : MemberAccessor
    {
        private static readonly Regex IndexRegex = new Regex(@"^\[?(?<index>\d+)\]?$", RegexOptions.Compiled);

        public EnumerableMemberAccessor(CompilationContext compilationContext) : base(compilationContext)
        {
        }

        public override bool TryAccessMember(BindingContext context, object instance, string memberName, out object result)
        {
            result = null;

            if (!(instance is IEnumerable<object> enumerable)) return false;

            var match = IndexRegex.Match(memberName);
            if (match.Success 
                && match.Groups["index"].Success 
                && int.TryParse(match.Groups["index"].Value, out var index)
                && TryGetElement(enumerable, index, out result))
            {
                return true;
            }

            memberName = ResolveMemberName(instance, memberName);
            if (!ContainsVariable(memberName)) return false;
            
            var contextVariable = context.GetContextVariable(memberName.Substring(1));
            return (contextVariable is int indexFromContext || contextVariable is string indexString && int.TryParse(indexString, out indexFromContext))
                   && TryGetElement(enumerable, indexFromContext, out result);
        }

        private static bool TryGetElement(IEnumerable<object> enumerable, int index, out object result)
        {
            result = null;
            var elementAtIndex = enumerable.ElementAtOrDefault(index);
            if (elementAtIndex == null)
            {
                return false;
            }

            result = elementAtIndex;
            return true;
        }
    }
}