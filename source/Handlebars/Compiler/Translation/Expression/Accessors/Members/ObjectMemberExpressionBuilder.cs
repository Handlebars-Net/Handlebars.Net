using System;

namespace HandlebarsDotNet.Compiler.Translation.Expression.Accessors.Members
{
    internal static class ObjectMemberExpressionBuilder
    {
        /// <summary>
        /// Builds a function to access a field or a property.
        /// </summary>
        /// <param name="instanceType">Type of the instance.</param>
        /// <param name="memberName">Name of the member to access.</param>
        /// <returns>Function to access a field or a property, or null if the member does not exists.</returns>
        public static Func<object, object> BuildMemberGetterFunc(Type instanceType, string memberName)
        {
            var memberType = GetMemberReturnType(instanceType, memberName);
            if(memberType == null) { return null; } //This means the memberName is not a field nor a property.

            var objInstanceParam = System.Linq.Expressions.Expression.Parameter(typeof(object), "objInstance");
            var castInstanceExpression = System.Linq.Expressions.Expression.Convert(objInstanceParam, instanceType);
            var instanceParam = System.Linq.Expressions.Expression.Parameter(instanceType, "instance");

            var propertyCallExpression = System.Linq.Expressions.Expression.PropertyOrField(instanceParam, memberName);
            var valueParam = System.Linq.Expressions.Expression.Parameter(memberType, "value");
            var castToObjectExpression = System.Linq.Expressions.Expression.Convert(valueParam, typeof(object));

            var returnParam = System.Linq.Expressions.Expression.Parameter(typeof(object), "returnValue");
            var returnTarget = System.Linq.Expressions.Expression.Label(typeof(object));
            var returnLabel = System.Linq.Expressions.Expression.Label(returnTarget,
                System.Linq.Expressions.Expression.Default(typeof(object)));

            var body = System.Linq.Expressions.Expression.Block(
                new[] { instanceParam, valueParam, returnParam },
                System.Linq.Expressions.Expression.Assign(instanceParam, castInstanceExpression),
                System.Linq.Expressions.Expression.Assign(valueParam, propertyCallExpression),
                System.Linq.Expressions.Expression.Assign(returnParam, castToObjectExpression),
                System.Linq.Expressions.Expression.Return(returnTarget, returnParam, typeof(object)),
                returnLabel
                );

            var func = System.Linq.Expressions.Expression.Lambda<Func<object, object>>(body, objInstanceParam).Compile();
            return func;
        }

        private static Type GetMemberReturnType(Type classType, string memberName)
        {
            var propertyInfo = classType.GetProperty(memberName);

            if (propertyInfo == null)
            {
                var fieldInfo = classType.GetField(memberName);

                if (fieldInfo == null)
                {
                    return null;
                }

                var fieldType = fieldInfo.FieldType;
                return fieldType;
            }

            var propertyType = propertyInfo.PropertyType;
            return propertyType;
        }
    }
}
