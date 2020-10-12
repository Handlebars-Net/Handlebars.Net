using System;
using System.Dynamic;

namespace HandlebarsDotNet.MemberAccessors
{
    internal class DynamicMemberAccessor : IMemberAccessor
    {
        public bool TryGetValue(object instance, Type instanceType, string memberName, out object value)
        {
            value = null;
            //crude handling for dynamic objects that don't have metadata
            var metaObjectProvider = (IDynamicMetaObjectProvider) instance;

            try
            {
                value = GetProperty(metaObjectProvider, memberName);
                return value != null;
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