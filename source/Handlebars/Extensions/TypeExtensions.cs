using System;
using System.Reflection;

namespace HandlebarsDotNet
{
    internal static class TypeExtensions
    {
        public static bool IsAssignableToGenericType(this Type givenType, Type genericType, out Type resolvedType)
        {
            while (true)
            {
                var interfaceTypes = givenType.GetInterfaces();

                for (var index = 0; index < interfaceTypes.Length; index++)
                {
                    resolvedType = interfaceTypes[index];
                    if (resolvedType.GetTypeInfo().IsGenericType && resolvedType.GetGenericTypeDefinition() == genericType) return true;
                }

                if (givenType.GetTypeInfo().IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                {
                    resolvedType = givenType;
                    return true;
                }

                var baseType = givenType.GetTypeInfo().BaseType;
                if (baseType == null)
                {
                    resolvedType = null;
                    return false;
                }

                givenType = baseType;
            }
        }
    }
}