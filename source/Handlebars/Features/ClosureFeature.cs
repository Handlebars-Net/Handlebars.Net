using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Expressions.Shortcuts;

namespace HandlebarsDotNet.Features
{
    internal class ClosureFeatureFactory : IFeatureFactory
    {
        public IFeature CreateFeature()
        {
            return new ClosureFeature();
        }
    }

    /// <summary>
    /// Extracts all <see cref="Expression.Constant(object)"/> items into precompiled closure allowing to compile static delegates
    /// </summary>
    [FeatureOrder(0)]
    public class ClosureFeature : IFeature
    {
        /// <summary>
        /// Parameter of actual closure
        /// </summary>
        internal ExpressionContainer<object[]> ClosureInternal { get; } = ExpressionShortcuts.Var<object[]>("closure");

        
        public ParameterExpression Closure => (ParameterExpression) ClosureInternal.Expression;

        /// <summary>
        /// Build-time closure store
        /// </summary>
        public TemplateClosure TemplateClosure { get; } = new TemplateClosure();
        
        internal LinkedList<ClosureFeature> Children { get; } = new LinkedList<ClosureFeature>(); 
        
        /// <summary>
        /// Middleware to use in order to apply transformation
        /// </summary>
        public IExpressionMiddleware ExpressionMiddleware { get; }

        
        public ClosureFeature()
        {
            ExpressionMiddleware = new ClosureExpressionMiddleware(TemplateClosure, ClosureInternal);
        }
        
        /// <inheritdoc />
        public void OnCompiling(ICompiledHandlebarsConfiguration configuration)
        {
            // noting to do here
        }

        /// <inheritdoc />
        public void CompilationCompleted()
        {
            TemplateClosure?.Build();

            foreach (var child in Children)
            {
                child.CompilationCompleted();
            }
        }

        private class ClosureExpressionMiddleware : IExpressionMiddleware
        {
            private readonly TemplateClosure _closure;
            private readonly ExpressionContainer<object[]> _closureArg;

            public ClosureExpressionMiddleware(TemplateClosure closure, ExpressionContainer<object[]> closureArg)
            {
                _closure = closure;
                _closureArg = closureArg;
            }

            public Expression Invoke(Expression expression)
            {
                var closureVisitor = new ClosureVisitor(_closureArg, _closure);
                var constantReducer = new ConstantsReducer();

                expression = closureVisitor.Visit(expression);
                return constantReducer.Visit(expression);
            }

            private class ClosureVisitor : ExpressionVisitor
            {
                private readonly TemplateClosure _templateClosure;
                private readonly ExpressionContainer<object[]> _templateClosureArg;

                public ClosureVisitor(ExpressionContainer<object[]> arg, TemplateClosure templateClosure)
                {
                    _templateClosure = templateClosure;
                    _templateClosureArg = arg;
                }

                protected override Expression VisitLambda<T>(Expression<T> node)
                {
                    var body = Visit(node.Body);
                    return node.Update(body ?? throw new InvalidOperationException("Cannot create closure"),
                        node.Parameters);
                }

                protected override Expression VisitConstant(ConstantExpression node)
                {
                    switch (node.Value)
                    {
                        case null:
                        case string _:
                            return node;

                        default:
                            if (node.Type.GetTypeInfo().IsPrimitive) return node;
                            break;
                    }

                    UnaryExpression unaryExpression;
                    if (_templateClosure.TryGetKeyByValue(node.Value, out var existingKey))
                    {
                        unaryExpression =
                            Expression.Convert(
                                Expression.ArrayIndex(_templateClosureArg, Expression.Constant(existingKey)),
                                node.Type);
                        return unaryExpression;
                    }

                    var key = _templateClosure.CurrentIndex;
                    _templateClosure[key] = node.Value;
                    var accessor = Expression.ArrayIndex(_templateClosureArg, Expression.Constant(key));
                    unaryExpression = Expression.Convert(accessor, node.Type);
                    return unaryExpression;
                }

                protected override Expression VisitMember(MemberExpression node)
                {
                    if (!(node.Expression is ConstantExpression constantExpression)) return base.VisitMember(node);

                    switch (node.Member)
                    {
                        case PropertyInfo property:
                        {
                            var value = property.GetValue(constantExpression.Value);
                            return VisitConstant(Expression.Constant(value, property.PropertyType));
                        }

                        case FieldInfo field:
                        {
                            var value = field.GetValue(constantExpression.Value);
                            return VisitConstant(Expression.Constant(value, field.FieldType));
                        }

                        default:
                        {
                            var constant = VisitConstant(constantExpression);
                            return node.Update(constant);
                        }
                    }
                }
            }
            
            private class ConstantsReducer : ExpressionVisitor
            {
                private readonly Dictionary<object, ConstantExpression> _expressions = new Dictionary<object, ConstantExpression>();
                
                protected override Expression VisitConstant(ConstantExpression node)
                {
                    if(node.Value != null && _expressions.TryGetValue(node.Value, out var storedNode))
                    {
                        return storedNode;
                    }

                    if (node.Value != null)
                    {
                        _expressions.Add(node.Value, node);
                    }
                    
                    return node;
                }
            }
        }
    }
}