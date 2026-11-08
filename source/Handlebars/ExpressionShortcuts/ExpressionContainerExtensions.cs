using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.ExpressionShortcuts
{
    internal static partial class ExpressionShortcuts
    {
        /// <summary>
        /// Creates strongly typed representation of the <see cref="Expression.Property(System.Linq.Expressions.Expression,System.String)"/>
        /// </summary>
        /// <param name="instance"/>
        /// <param name="propertyAccessor">Property accessor expression</param>
        /// <typeparam name="T">Expected type of resulting target <see cref="Expression"/></typeparam>
        /// <typeparam name="TV">Expected type of resulting <see cref="MemberExpression"/></typeparam>
        /// <returns><see cref="ExpressionContainer{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer<TV> Property<T, TV>(this ExpressionContainer<T> instance, Expression<Func<T, TV>> propertyAccessor)
        {
            var visitor = new ReplaceParameterVisitor(propertyAccessor.Parameters[0], instance);
            var combinedExpression = visitor.Visit(propertyAccessor.Body);
            return new ExpressionContainer<TV>(combinedExpression);
        }

        /// <summary>
        /// Creates <see cref="MethodCallExpression"/> or <see cref="InvocationExpression"/> based on <paramref name="invocationExpression"/>.
        /// Parameters are resolved based on actual passed parameters.
        /// </summary>
        /// <param name="instance"/>
        /// <param name="invocationExpression">Expression used to invoke the method.</param>
        /// <returns><see cref="Void"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer Call<T>(this ExpressionContainer<T> instance, Expression<Action<T>> invocationExpression)
        {
            var visitor = new ReplaceParameterVisitor(invocationExpression.Parameters[0], instance);
            var combinedExpression = visitor.Visit(invocationExpression.Body);
            return new ExpressionContainer(combinedExpression);
        }
        
        /// <summary>
        /// Creates <see cref="MethodCallExpression"/> or <see cref="InvocationExpression"/> based on <paramref name="invocationExpression"/>.
        /// Parameters are resolved based on actual passed parameters.
        /// </summary>
        /// <param name="instance"/>
        /// <param name="invocationExpression">Expression used to invoke the method.</param>
        /// <returns><see cref="ExpressionContainer{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer<TV> Call<T, TV>(this ExpressionContainer<T> instance, Expression<Func<T, TV>> invocationExpression)
        {
            var visitor = new ReplaceParameterVisitor(invocationExpression.Parameters[0], instance);
            var combinedExpression = visitor.Visit(invocationExpression.Body);
            return new ExpressionContainer<TV>(combinedExpression);
        }

        /// <summary>
        /// Creates assign <see cref="BinaryExpression"/>.
        /// Parameters are resolved based on actual passed parameters.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer<T> Assign<T>(this ExpressionContainer<T> target, ExpressionContainer<T> value)
        {
            return new ExpressionContainer<T>(Expression.Assign(target, value));
        }
        
        /// <summary>
        /// Creates assign <see cref="BinaryExpression"/>.
        /// Parameters are resolved based on actual passed parameters.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer<T> Assign<T>(this ExpressionContainer<T> target, T value)
        {
            return new ExpressionContainer<T>(Expression.Assign(target, Expression.Constant(value, typeof(T))));
        }
    }
}