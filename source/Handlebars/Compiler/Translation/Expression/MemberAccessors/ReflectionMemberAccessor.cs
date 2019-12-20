using System.Linq;
using System.Reflection;

namespace HandlebarsDotNet.Compiler
{
    internal class ReflectionMemberAccessor : MemberAccessor
    {
        public ReflectionMemberAccessor(CompilationContext compilationContext) : base(compilationContext)
        {
        }

        public override bool TryAccessMember(BindingContext context, object instance, string memberName, out object result)
        {
            result = null;

            var instanceType = instance.GetType();
            memberName = ResolveMemberName(instance, memberName);
            var members = instanceType.GetMember(memberName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (members.Length == 0) return false;

            var preferredMember = members.Length > 1
                ? members.FirstOrDefault(m => m.Name == memberName) ?? members[0]
                : members[0];
            
            switch (preferredMember)
            {
                case PropertyInfo propertyInfo:
                    result = propertyInfo.GetValue(instance, null);
                    return true;
                
                case FieldInfo fieldInfo:
                    result = fieldInfo.GetValue(instance);
                    return true;
                
                default:
                    return false;
            }
        }
    }
}