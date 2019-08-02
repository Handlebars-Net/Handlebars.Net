using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;

namespace Handlebars
{
    internal static class DynamicMetaObjectHelper
    {
        internal static object GetProperty(IDynamicMetaObjectProvider target, string name)
        {
            var self = (Expression)Expression.Constant(target);
            var metaObject = target.GetMetaObject(self);
            var binder = (GetMemberBinder)Binder.GetMember(CSharpBinderFlags.None, name, target.GetType(), new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
            var member = metaObject.BindGetMember(binder);
            var final = Expression.Block(
                Expression.Label(CallSiteBinder.UpdateLabel),
                member.Expression
            );
            var param = Expression.Parameter(typeof(object));
            var lambda = Expression.Lambda(final, param);
            var @delegate = lambda.Compile();
            return @delegate.DynamicInvoke(target);
        }
    }
}
