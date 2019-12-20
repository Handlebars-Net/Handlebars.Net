using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HandlebarsDotNet.Compiler
{
    internal class GenericDictionaryMemberAccessor : MemberAccessor
    {
        public GenericDictionaryMemberAccessor(CompilationContext compilationContext) : base(compilationContext)
        {
        }

        public override bool TryAccessMember(BindingContext context, object instance, string memberName, out object result)
        {
            result = null;

            // Check if the instance is IDictionary<,>
            var instanceType = instance.GetType();
            var iDictInstance = FirstGenericDictionaryTypeInstance(instanceType);
            if (iDictInstance == null) return false;

            var genericArgs = iDictInstance.GetGenericArguments();
            memberName = ResolveMemberName(instance, memberName);
            object key = ContainsVariable(memberName)
                ? context.GetContextVariable(memberName.Substring(1))
                : memberName;

            if (genericArgs.Length > 0 && genericArgs[0] != key.GetType())
            {
                // Dictionary key type isn't a string, so attempt to convert.
                try
                {
                    key = Convert.ChangeType(key, genericArgs[0], CultureInfo.CurrentCulture);
                }
                catch (Exception)
                {
                    // Can't convert to key type.
                    return false;
                }
            }

            var containsKeyMethod = GetMethod(instanceType, "ContainsKey");
            if (containsKeyMethod == null)
            {
                throw new MethodAccessException("Method ContainsKey not found");
            }

            var parameters = new[] {key};
            if (!(bool) containsKeyMethod.Invoke(instance, parameters))
                return false; // Key doesn't exist.

            var itemProperty = GetMethod(instanceType, "get_Item");
            if (itemProperty == null)
            {
                throw new MethodAccessException("Property Item not found");
            }

            result = itemProperty.Invoke(instance, parameters);
            return true;
        }

        private static MethodInfo GetMethod(Type instanceType, string methodName)
        {
            var methodInfo = instanceType.GetMethod(methodName);

            if (methodInfo == null)
            {
                // Support implicit interface impl.
                methodInfo = instanceType.GetTypeInfo().DeclaredMethods.FirstOrDefault(m =>
                    m.IsPrivate && m.Name.StartsWith("System.Collections.Generic.IDictionary") &&
                    m.Name.EndsWith(methodName));
            }

            return methodInfo;
        }

        private static Type FirstGenericDictionaryTypeInstance(Type instanceType)
        {
            return instanceType.GetInterfaces()
                .FirstOrDefault(i =>
#if netstandard
                    i.GetTypeInfo().IsGenericType
#else
                    i.IsGenericType
#endif
                    &&
                    (
                        i.GetGenericTypeDefinition() == typeof(IDictionary<,>)
                    )
                );
        }
    }
}