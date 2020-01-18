using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace HandlebarsDotNet.Compiler
{
    internal class BlockBody : List<Expression>
    {
            
    }
    
    internal class ExpressionContainer
    {
        private readonly Expression _expression;

        public ExpressionContainer(Expression expression) => _expression = expression;

        public virtual Expression Expression => _expression;

        public ExpressionContainer<T> Typed<T>() => new ExpressionContainer<T>(Expression);
        
        public ExpressionContainer<bool> Is<TV>() => new ExpressionContainer<bool>(Expression.TypeIs(Expression, typeof(TV)));
        public ExpressionContainer<TV> As<TV>() => new ExpressionContainer<TV>(Expression.TypeAs(Expression, typeof(TV)));
        public ExpressionContainer<TV> Cast<TV>() => new ExpressionContainer<TV>(Expression.Convert(Expression, typeof(TV)));
        
        public static implicit operator Expression(ExpressionContainer expressionContainer) => expressionContainer.Expression;
        public static implicit operator ExpressionContainer(Expression expression) => new ExpressionContainer(expression);
    }
      
    /// <summary>
    /// Provides strongly typed container for <see cref="Expression"/>.
    /// </summary>
    /// <remarks>Used to trick C# compiler in cases like <see cref="E.Call"/> in order to pass value to target method.</remarks>
    /// <typeparam name="T">Type of expected <see cref="Expression"/> result value.</typeparam>
    internal class ExpressionContainer<T> : ExpressionContainer
    {
        public static implicit operator T(ExpressionContainer<T> expressionContainer) => default(T);

        public ExpressionContainer(Expression expression) : base(expression)
        {
        }
    }
    
    /// <summary>
    /// Stands for <see cref="Expression"/> shortcuts.
    /// </summary>
    internal static class E
    {
        /// <summary>
        /// Creates strongly typed representation of the <paramref name="expression"/>
        /// </summary>
        /// <remarks>If <paramref name="expression"/> is <c>null</c> returns  result of <see cref="Null{T}"/></remarks>
        /// <param name="expression"><see cref="Expression"/> to wrap</param>
        /// <typeparam name="T">Expected type of resulting <see cref="Expression"/></typeparam>
        /// <returns><see cref="ExpressionContainer{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer<T> Arg<T>(Expression expression) => expression == null ? Null<T>() : new ExpressionContainer<T>(expression);

        /// <summary>
        /// Creates strongly typed representation of the <paramref name="expression"/>.
        /// </summary>
        /// <remarks>If <paramref name="expression"/> is <c>null</c> returns  result of <see cref="Null{T}"/></remarks>
        /// <param name="expression"><see cref="Expression"/> to wrap</param>
        /// <typeparam name="T">Expected type of resulting <see cref="Expression"/></typeparam>
        /// <returns><see cref="ExpressionContainer{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer<T> Arg<T>(Expression<T> expression) => expression == null ? Null<T>() : new ExpressionContainer<T>(expression);
        
        /// <summary>
        /// Creates strongly typed representation of the <paramref name="expression"/> and performs <see cref="Expression.Convert(System.Linq.Expressions.Expression,System.Type)"/> on it.
        /// </summary>
        /// <remarks>If <paramref name="expression"/> is <c>null</c> returns  result of <see cref="Null{T}"/></remarks>
        /// <param name="expression"><see cref="Expression"/> to wrap</param>
        /// <typeparam name="T">Expected type of resulting <see cref="Expression"/></typeparam>
        /// <returns><see cref="ExpressionContainer{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer<T> Cast<T>(Expression expression) => expression == null ? Null<T>() : new ExpressionContainer<T>(Expression.Convert(expression, typeof(T)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Expression> ReplaceValuesOf<T>(IEnumerable<Expression> instance, Expression newValue)
        {
            var targetType = typeof(T);
            return instance.Select(value => targetType.IsAssignableFrom(value.Type)
                ? newValue 
                : value);
        }
        
        public static IEnumerable<Expression> ReplaceParameters(IEnumerable<Expression> instance, IList<Expression> newValue)
        {
            return newValue.Count != 0 
                ? PerformReplacement() 
                : instance;

            IEnumerable<Expression> PerformReplacement()
            {
                var visitor = new ParameterReplacerVisitor(newValue);
                return instance.Where(o => o != null).Select(expression => visitor.Visit(expression));
            }
        }

        /// <summary>
        /// Creates strongly typed representation of the <see cref="Expression.Variable(System.Type, System.String)"/>
        /// </summary>
        /// <param name="name">Variable name. Corresponds to type name if omitted.</param>
        /// <typeparam name="T">Expected type of resulting <see cref="ParameterExpression"/></typeparam>
        /// <returns><see cref="ExpressionContainer{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer<T> Var<T>(string name = null)
        {
            return new ExpressionContainer<T>(Expression.Variable(typeof(T), name ?? typeof(T).Name));
        }
        
        /// <summary>
        /// Creates strongly typed representation of the <see cref="Expression.Parameter(System.Type, System.String)"/>
        /// </summary>
        /// <param name="name">Variable name. Corresponds to type name if omitted.</param>
        /// <typeparam name="T">Expected type of resulting <see cref="ParameterExpression"/></typeparam>
        /// <returns><see cref="ExpressionContainer{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer<T> Parameter<T>(string name = null)
        {
            return new ExpressionContainer<T>(Expression.Parameter(typeof(T), name ?? typeof(T).Name));
        }


        /// <summary>
        /// Creates strongly typed representation of the <see cref="Expression.Property(System.Linq.Expressions.Expression,System.String)"/>
        /// </summary>
        /// <param name="instance">Variable name. Corresponds to type name if omitted.</param>
        /// <param name="propertyLambda">Property accessor expression</param>
        /// <typeparam name="T">Expected type of resulting target <see cref="Expression"/></typeparam>
        /// <typeparam name="TV">Expected type of resulting <see cref="MemberExpression"/></typeparam>
        /// <returns><see cref="ExpressionContainer{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer<TV> Property<T, TV>(Expression instance, Expression<Func<T, TV>> propertyLambda)
        {
            return Arg<TV>(ProcessPropertyLambda(instance, propertyLambda));
        }

        /// <summary>
        /// Creates strongly typed representation of the <see cref="Expression.Property(System.Linq.Expressions.Expression,System.String)"/>
        /// </summary>
        /// <param name="instance">Variable name. Corresponds to type name if omitted.</param>
        /// <param name="propertyLambda">Property accessor expression</param>
        /// <param name="propertyName"/>
        /// <typeparam name="TV">Expected type of resulting <see cref="MemberExpression"/></typeparam>
        /// <returns><see cref="ExpressionContainer{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer<TV> Property<TV>(Expression instance, string propertyName)
        {
            return Arg<TV>(Expression.Property(instance, propertyName));
        }

        /// <summary>
        /// Creates strongly typed representation of the <see cref="Expression.NewArrayInit(System.Type,System.Collections.Generic.IEnumerable{System.Linq.Expressions.Expression})"/>
        /// </summary>
        /// <param name="items">Items for the new array</param>
        /// <typeparam name="T">Expected type of resulting <see cref="NewArrayExpression"/></typeparam>
        /// <returns><see cref="ExpressionContainer{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer<T[]> Array<T>(IEnumerable<Expression> items)
        {
            return Arg<T[]>(Expression.NewArrayInit(typeof(T), items));
        }

        /// <summary>
        /// Creates <see cref="MethodCallExpression"/> or <see cref="InvocationExpression"/> based on <paramref name="invocationExpression"/>.
        /// Parameters are resolved based on actual passed parameters.
        /// </summary>
        /// <param name="invocationExpression">Expression used to invoke the method.</param>
        /// <returns><see cref="Void"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer Call(Expression<Action> invocationExpression)
        {
            return new ExpressionContainer(ProcessCallLambda(invocationExpression));
        }
        
        /// <summary>
        /// Creates <see cref="MethodCallExpression"/> or <see cref="InvocationExpression"/> based on <paramref name="invocationExpression"/>.
        /// Parameters are resolved based on actual passed parameters.
        /// </summary>
        /// <param name="invocationExpression">Expression used to invoke the method.</param>
        /// <returns><see cref="Void"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer Call(ExpressionContainer instance, ExpressionContainer<MethodInfo> method, params ExpressionContainer[] parameters)
        {
            var methodCallExpression = (MethodCallExpression) ProcessCall(method, instance);
            return methodCallExpression.Update(methodCallExpression.Object, parameters.Cast<Expression>());
        }

        /// <summary>
        /// Creates <see cref="MethodCallExpression"/> or <see cref="InvocationExpression"/> based on <paramref name="invocationExpression"/>.
        /// Parameters are resolved based on actual passed parameters.
        /// </summary>
        /// <param name="invocationExpression">Expression used to invoke the method.</param>
        /// <returns><see cref="ExpressionContainer{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer<T> Call<T>(Expression<Func<T>> invocationExpression)
        {
            return Arg<T>(ProcessCallLambda(invocationExpression));
        }

        /// <summary>
        /// Creates <see cref="NewExpression"/>. Parameters for constructor and constructor itself are resolved based <paramref name="invocationExpression"/>.
        /// </summary>
        /// <param name="invocationExpression">Expression used to invoke the method.</param>
        /// <returns><see cref="ExpressionContainer{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer<T> New<T>(Expression<Func<T>> invocationExpression)
        {
            return Arg<T>(ProcessCallLambda(invocationExpression));
        }
        
        /// <summary>
        /// Creates <see cref="NewExpression"/> using default constructor.
        /// </summary>
        /// <returns><see cref="ExpressionContainer{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer<T> New<T>() where T: new()
        {
            return Arg<T>(Expression.New(typeof(T).GetConstructor(new Type[0])));
        }
        
        /// <summary>
        /// Provides fluent interface for <see cref="BlockExpression"/> creation
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BlockBuilder Block()
        {
            return new BlockBuilder();
        }

        /// <summary>
        /// Creates strongly typed representation of <c>null</c>.
        /// </summary>
        /// <typeparam name="T">Expected type of resulting <see cref="Expression"/></typeparam>
        /// <returns><see cref="ExpressionContainer{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer<T> Null<T>()
        {
            return Arg<T>(Expression.Convert(Expression.Constant(null), typeof(T)));
        }
        
        /// <summary>
        /// Creates strongly typed representation of <c>null</c>.
        /// </summary>
        /// <typeparam name="T">Expected type of resulting <see cref="Expression"/></typeparam>
        /// <returns><see cref="ExpressionContainer{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer Null(Type type)
        {
            return new ExpressionContainer(Expression.Convert(Expression.Constant(null), type));
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
            return new ExpressionContainer(ProcessCallLambda(invocationExpression, instance));
        }
        
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
            return Property(instance.Expression, propertyAccessor);
        }
        
        /// <summary>
        /// Creates strongly typed representation of the <see cref="Expression.Property(System.Linq.Expressions.Expression,System.String)"/>
        /// </summary>
        /// <param name="instance"/>
        /// <param name="propertyAccessor">Property accessor expression</param>
        /// <typeparam name="T">Expected type of resulting target <see cref="Expression"/></typeparam>
        /// <typeparam name="TV">Expected type of resulting <see cref="MemberExpression"/></typeparam>
        /// <returns><see cref="ExpressionContainer{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer<TV> Property<TV>(this ExpressionContainer instance, string propertyName)
        {
            return Property<TV>(instance.Expression, propertyName);
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
            return Arg<TV>(ProcessCallLambda(invocationExpression, instance));
        }

        /// <summary>
        /// Creates assign <see cref="BinaryExpression"/>.
        /// Parameters are resolved based on actual passed parameters.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer Assign<T>(this ExpressionContainer<T> target, ExpressionContainer<T> value)
        {
            return new ExpressionContainer(Expression.Assign(target, value));
        }
        
        /// <summary>
        /// Creates assign <see cref="BinaryExpression"/>.
        /// Parameters are resolved based on actual passed parameters.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionContainer Assign<T>(this ExpressionContainer<T> target, Expression value)
        {
            return new ExpressionContainer(Expression.Assign(target, value));
        }
        
        /// <summary>
        /// Creates ternary assignment like <code>target = condition ? ifTrue : ifFalse</code>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expression TernaryAssign<T>(this ExpressionContainer<T> target, ExpressionContainer<bool> condition, ExpressionContainer<T> ifTrue, ExpressionContainer<T> ifFalse)
        {
            return Expression.IfThenElse(
                condition.Expression,
                Expression.Assign(target, ifTrue),
                Expression.Assign(target, ifFalse));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static MemberExpression ProcessPropertyLambda(Expression instance, LambdaExpression propertyLambda)
        {
            var member = propertyLambda.Body as MemberExpression;
            if (member == null) throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null) throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");
            
            return Expression.Property(instance, propInfo);
        }

        private static Expression ProcessCallLambda(LambdaExpression propertyLambda, Expression instance = null)
        {
            return ProcessCall(propertyLambda.Body, instance);
        }
        
        private static Expression ProcessCall(Expression propertyLambda, Expression instance = null)
        {
            var parameters = instance != null ? new[] { instance } : new Expression[0];

            switch (propertyLambda)
            {
                case NewExpression newExpression:
                    return Expression.New(newExpression.Constructor, ExtractArguments(newExpression.Arguments));

                case MethodCallExpression member:
                    var methodInfo = member.Method;
                    IEnumerable<Expression> methodCallArguments = member.Arguments;
                    var inst = ReplaceParameters(new[] {member.Object}, parameters).SingleOrDefault();
                    methodCallArguments = ReplaceParameters(methodCallArguments, parameters);
                    methodCallArguments = methodCallArguments.Select(ExtractArgument);
                    var memberObject = methodInfo.IsStatic 
                        ? null : Expression.Convert(inst, methodInfo.DeclaringType);
                    return Expression.Call(memberObject, methodInfo, methodCallArguments);

                case InvocationExpression invocationExpression:
                    return invocationExpression.Update(
                        invocationExpression.Expression,
                        ExtractArguments(invocationExpression.Arguments)
                    );

                default:
                    return propertyLambda;
            }
        }

        private static IReadOnlyCollection<Expression> ExtractArguments(IReadOnlyCollection<Expression> exprs)
        {
            var result = new Expression[exprs.Count];
            if (exprs is IList<Expression> list)
            {
                for (var index = 0; index < list.Count; index++)
                {
                    result[index] = ExtractArgument(list[index]);
                }

                return result;
            }
            else
            {
                int index = 0;
                foreach (var expr in exprs)
                {
                    result[index++] = ExtractArgument(expr);
                }
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Expression ExtractArgument(Expression expr)
        {
            var value = ExtractFromExpression(expr);

            if (value is Expression expression) return expression;
            if (value is ExpressionContainer expressionContainer) return expressionContainer.Expression;
            return Expression.Constant(value);
        }

        private static object ExtractFromExpression(Expression expression)
        {
            while (true)
            {
                switch (expression)
                {
                    case MethodCallExpression methodCall:
                        return Expression.Lambda(methodCall).Compile().DynamicInvoke();

                    case UnaryExpression unaryExpression:
                        switch (unaryExpression.NodeType)
                        {
                            case ExpressionType.Convert:
                                if (typeof(ExpressionContainer).IsAssignableFrom(unaryExpression.Operand.Type))
                                {
                                    var operand = ExtractArgument(unaryExpression.Operand);
                                    if (operand.Type != unaryExpression.Type)
                                    {
                                        return Expression.Convert(operand, unaryExpression.Type);
                                    }

                                    if (operand.NodeType == ExpressionType.Call || operand.NodeType == ExpressionType.Invoke || operand.NodeType == ExpressionType.Lambda)
                                    {
                                        return operand;
                                    }

                                    expression = operand;
                                    continue;
                                }
                                
                                if (unaryExpression.Type != unaryExpression.Operand.Type)
                                {
                                    return expression;
                                }
                                
                                expression = unaryExpression.Operand;
                                continue;

                            default:
                                expression = unaryExpression.Operand;
                                continue;
                        }

                    case LambdaExpression lambda:
                        return ProcessCallLambda(lambda);

                    case MemberExpression memberExpression:
                        switch (memberExpression.Expression)
                        {
                            case ConstantExpression constant:
                                var constantValue = constant.Value;
                                var value = constantValue.GetType().GetField(memberExpression.Member.Name)?.GetValue(constantValue);
                                if (value is ExpressionContainer) return value;
                                return Expression.Convert(Expression.Constant(value), memberExpression.Type);

                            default: 
                                return memberExpression;
                        }

                    default:
                        return expression;
                }
            }
        }

        internal class BlockBuilder: ExpressionContainer
        {
            private readonly List<Expression> _expressions;
            private readonly List<ParameterExpression> _parameters;

            public BlockBuilder() : base(null)
            {
                _expressions = new List<Expression>();
                _parameters = new List<ParameterExpression>();
            }

            /// <summary>
            /// Adds parameter to <see cref="BlockExpression"/>
            /// </summary>
            /// <exception cref="ArgumentException"><paramref name="expression"/> is not <see cref="ParameterExpression"/></exception>
            public BlockBuilder Parameter(Expression expression)
            {
                if (expression is ParameterExpression parameterExpression) return Parameter(parameterExpression);
                
                throw new ArgumentException("is not ParameterExpression", nameof(expression));
            }

            /// <summary>
            /// Adds parameter to <see cref="BlockExpression"/> with initial assignment
            /// </summary>
            /// <exception cref="ArgumentException"><paramref name="expression"/> is not <see cref="ParameterExpression"/></exception>
            public BlockBuilder Parameter<TV>(ExpressionContainer<TV> expression, ExpressionContainer<TV> value)
            {
                if (!(expression.Expression is ParameterExpression parameterExpression))
                    throw new ArgumentException("is not ParameterExpression", nameof(expression));
                
                _parameters.Add(parameterExpression);
                _expressions.Add(expression.Assign(value));
                return this;
            }
            
            /// <summary>
            /// Adds parameter to <see cref="BlockExpression"/> with initial assignment
            /// </summary>
            /// <exception cref="ArgumentException"><paramref name="expression"/> is not <see cref="ParameterExpression"/></exception>
            public BlockBuilder Parameter<TV>(ExpressionContainer<TV> expression, Expression value)
            {
                if (!(expression.Expression is ParameterExpression parameterExpression))
                    throw new ArgumentException("is not ParameterExpression", nameof(expression));
                
                _parameters.Add(parameterExpression);
                _expressions.Add(expression.Assign(value));
                return this;
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
            /// Adds new "line" to <see cref="BlockExpression"/>
            /// </summary>
            public BlockBuilder Line<TV>(ExpressionContainer<TV> e)
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
            public ExpressionContainer<T> Invoke<T>(params ExpressionContainer[] parameters)
            {
                return Arg<T>(Expression.Lambda(Expression, parameters.Select(o => (ParameterExpression) o.Expression)));
            }

            public override Expression Expression => Expression.Block(_parameters, _expressions);
        }
    }

    internal class ParameterReplacerVisitor : ExpressionVisitor
    {
        private readonly ICollection<Expression> _replacements;
        private readonly bool _addIfMiss;

        public ParameterReplacerVisitor(ICollection<Expression> replacements, bool addIfMiss = false)
        {
            _replacements = replacements;
            _addIfMiss = addIfMiss;
        }
            
        protected override Expression VisitParameter(ParameterExpression node)
        {
            var replacement = _replacements.FirstOrDefault(o => o.Type == node.Type);
            if (replacement == null || replacement == node)
            {
                if (_addIfMiss)
                {
                    _replacements.Add(node);
                }
                return base.VisitParameter(node);
            }
            return base.Visit(replacement);
        }
    }
}