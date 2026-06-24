using System.Linq.Expressions;

namespace HandlebarsDotNet.ExpressionShortcuts
{
    /// <summary>
    /// Wrapper around of <see cref="System.Linq.Expressions.Expression"/> to provide addition functionality
    /// </summary>
    internal class ExpressionContainer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        public ExpressionContainer(Expression expression) => Expression = expression;

        /// <summary>
        /// Return the underling <see cref="System.Linq.Expressions.Expression"/>
        /// </summary>
        public virtual Expression Expression { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expressionContainer"></param>
        /// <returns></returns>
        public static implicit operator Expression(ExpressionContainer expressionContainer) => expressionContainer.Expression;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static implicit operator ExpressionContainer(Expression expression) => new ExpressionContainer(expression);
    }
    
    /// <summary>
    /// Provides strongly typed container for <see cref="Expression"/>.
    /// </summary>
    /// <remarks>Used to trick C# compiler in cases like <see cref="ExpressionShortcuts.Call"/> in order to pass value to target method.</remarks>
    /// <typeparam name="T">Type of expected <see cref="Expression"/> result value.</typeparam>
    internal class ExpressionContainer<T> : ExpressionContainer
    {
        /// <summary>
        /// Used to trick C# compiler
        /// </summary>
        public static implicit operator T(ExpressionContainer<T> _0) => default(T)!;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        public ExpressionContainer(Expression expression) : base(expression)
        {
        }
    }
}