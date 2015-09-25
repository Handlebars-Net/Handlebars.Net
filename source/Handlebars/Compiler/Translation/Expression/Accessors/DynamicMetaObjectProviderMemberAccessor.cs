using System;
using System.Dynamic;

namespace HandlebarsDotNet.Compiler.Translation.Expression.Accessors
{
    /// <summary>
    /// Member accessor for crude handling of dynamic objects that don't have metadata
    /// </summary>
    internal sealed class DynamicMetaObjectProviderMemberAccessor : IMemberAccessor
    {
        /// <summary>
        /// Determines if the memberName passed later should be the memberName or the memberName.
        /// </summary>
        public bool RequiresResolvedMemberName { get { return true; } }

        /// <summary>
        /// Determines if a member can be accessed using the current accessor.
        /// </summary>
        /// <param name="instance">Instance of the object to access.</param>
        /// <returns></returns>
        public bool CanHandle(object instance)
        {
            var instanceType = instance.GetType();
            var canHandle = typeof (IDynamicMetaObjectProvider).IsAssignableFrom(instanceType);

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
            try
            {
                var value = GetProperty(instance, memberName);
                return value;
            }
            catch
            {
                return new UndefinedBindingResult();
            }
        }

        private static object GetProperty(object target, string name)
        {
            var site = System.Runtime.CompilerServices.CallSite<Func<System.Runtime.CompilerServices.CallSite, object, object>>
                .Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, name, target.GetType(), new[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) }));
            var value = site.Target(site, target);

            return value;
        }
    }
}
