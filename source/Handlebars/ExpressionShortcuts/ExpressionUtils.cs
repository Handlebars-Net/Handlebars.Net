using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.ExpressionShortcuts
{
    /// <summary>
    /// 
    /// </summary>
    internal static class ExpressionUtils
    {
        /// <summary>
        /// Visits <paramref name="expressions"/> and replaces <see cref="ParameterExpression"/> by <paramref name="newValues"/> performing match by <see cref="Expression.Type"/>
        /// </summary>
        private static IEnumerable<Expression> ReplaceParameters(IEnumerable<Expression?> expressions, IList<Expression> newValues)
        {
            return (newValues.Count != 0 
                ? PerformReplacement()
                : expressions)!;

            IEnumerable<Expression> PerformReplacement()
            {
                var visitor = new ParameterReplacerVisitor(newValues);
                return expressions.Where(o => o != null).Select(visitor.Visit)!;
            }
        }
        
        /// <summary>
        /// Visits <paramref name="expression"/> and replaces <see cref="ParameterExpression"/> by <paramref name="newValues"/> performing match by <see cref="Expression.Type"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NotNullIfNotNull(nameof(expression))]
        private static Expression? ReplaceParameters(Expression? expression, params Expression?[] newValues)
        {
            var visitor = new ParameterReplacerVisitor(newValues);
            return visitor.Visit(expression);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Expression ProcessPropertyLambda(Expression instance, LambdaExpression propertyLambda)
        {
            if (propertyLambda.Body is not MemberExpression member)
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");

            if (member.Member is not PropertyInfo)
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");

            return ReplaceParameters(ExtractArgument(member), instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Expression ProcessCallLambda(LambdaExpression propertyLambda, Expression? instance = null)
        {
            return ProcessCall(propertyLambda.Body, instance);
        }
        
        internal static Expression ProcessCall(Expression propertyLambda, Expression? instance = null)
        {
            switch (propertyLambda)
            {
                case NewExpression newExpression:
                    return newExpression.Update(ExtractArguments(newExpression.Arguments));

                case MethodCallExpression member:
                    var methodInfo = member.Method;
                    var parameters = instance != null ? new[] { instance } : new Expression[0];
                    instance = ReplaceParameters(new[] {member.Object}, parameters).SingleOrDefault();
                    IEnumerable<Expression> methodCallArguments = member.Arguments;
                    methodCallArguments = ReplaceParameters(methodCallArguments, parameters).Select(ExtractArgument);
                    var memberObject = methodInfo.IsStatic 
                        ? null : Expression.Convert(instance!, methodInfo.DeclaringType!);

                    return Expression.Call(memberObject, methodInfo, methodCallArguments);

                case InvocationExpression invocationExpression:
                    return invocationExpression.Update(
                        invocationExpression.Expression,
                        ExtractArguments(invocationExpression.Arguments)
                    );

                default:
                    return ReplaceParameters(ExtractArgument(propertyLambda), instance);
            }
        }

        private static IReadOnlyCollection<Expression> ExtractArguments(IReadOnlyCollection<Expression> expressions)
        {
            var result = new Expression[expressions.Count];
            if (expressions is IList<Expression> list)
            {
                for (var index = 0; index < list.Count; index++)
                {
                    result[index] = ExtractArgument(list[index]);
                }
            }
            else
            {
                int index = 0;
                foreach (var expr in expressions)
                {
                    result[index++] = ExtractArgument(expr);
                }
            }

            return result;
        }
        
        private static readonly ExpressionExtractorVisitor ExtractorVisitor = new ExpressionExtractorVisitor();

        private static Expression ExtractArgument(Expression expr)
        {
            return ExtractorVisitor.Visit(expr);
        }
    }
}