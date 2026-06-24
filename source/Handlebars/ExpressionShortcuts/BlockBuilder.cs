using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HandlebarsDotNet.ExpressionShortcuts
{
    /// <summary>
    /// Shortcut for <see cref="BlockExpression"/>
    /// </summary>
    internal class BlockBuilder: ExpressionContainer
    {
        private readonly Type? _returnType;
        private readonly List<Expression> _expressions;
        private readonly HashSet<ParameterExpression> _parameters;

        internal BlockBuilder(Type? returnType) : base(Expression.Empty())
        {
            _returnType = returnType;
            _expressions = new List<Expression>();
            _parameters = new HashSet<ParameterExpression>();
        }

        /// <inheritdoc />
        public override Expression Expression => 
            _returnType == null 
                ? Expression.Block(_parameters, _expressions) 
                : Expression.Block(_returnType, _parameters, _expressions);

        /// <summary>
        /// Adds parameter to <see cref="BlockExpression"/>
        /// </summary>
        public BlockBuilder Parameter<T>(out ExpressionContainer<T> parameter)
        {
            var expression = Expression.Parameter(typeof(T));
            parameter = ExpressionShortcuts.Arg<T>(expression);
            return Parameter(expression);
        }

        /// <summary>
        /// Adds parameter to <see cref="BlockExpression"/>
        /// </summary>
        public BlockBuilder Parameter(ParameterExpression e)
        {
            _parameters.Add(e);
            return this;
        }
            
        /// <summary>
        /// Adds new "line" to <see cref="BlockExpression"/>
        /// </summary>
        public BlockBuilder Line(Expression e)
        {
            _expressions.Add(e);
            return this;
        }

        /// <summary>
        /// Adds multiple new "lines" to <see cref="BlockExpression"/>
        /// </summary>
        public BlockBuilder Lines(IEnumerable<Expression> e)
        {
            _expressions.AddRange(e);
            return this;
        }

        /// <summary>
        /// Creates <see cref="InvocationExpression"/> out of current <see cref="BlockExpression"/>.
        /// </summary>
        public Expression<T> Lambda<T>(IEnumerable<ParameterExpression> parameters) where T : class
        {
            return Expression.Lambda<T>(Expression, parameters);
        }
    }
}