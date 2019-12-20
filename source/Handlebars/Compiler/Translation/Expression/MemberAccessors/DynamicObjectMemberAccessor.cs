using System;
using System.Dynamic;

namespace HandlebarsDotNet.Compiler
{
    internal class DynamicObjectMemberAccessor : MemberAccessor
    {
        public DynamicObjectMemberAccessor(CompilationContext compilationContext) : base(compilationContext)
        {
        }

        public override bool TryAccessMember(BindingContext context, object instance, string memberName, out object result)
        {
            result = null;

            if (!(instance is IDynamicMetaObjectProvider metaObjectProvider)) return false;

            memberName = ResolveMemberName(instance, memberName);
            memberName = ContainsVariable(memberName)
                ? context.GetContextVariable(memberName.Substring(1)) as string ?? memberName
                : memberName;
            
            try
            {
                result = GetProperty(metaObjectProvider, memberName);
                return result != null;
            }
            catch
            {
                return false;
            }
        }
        
        private static object GetProperty(object target, string name)
        {
            var site = System.Runtime.CompilerServices.CallSite<Func<System.Runtime.CompilerServices.CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, name, target.GetType(), new[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) }));
            return site.Target(site, target);
        }
    }
}