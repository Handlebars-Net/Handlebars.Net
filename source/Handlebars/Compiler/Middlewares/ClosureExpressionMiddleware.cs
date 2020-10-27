using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using static Expressions.Shortcuts.ExpressionShortcuts;

namespace HandlebarsDotNet.Compiler.Middlewares
{
    internal class ClosureExpressionMiddleware : IExpressionMiddleware
    {
        public Expression<T> Invoke<T>(Expression<T> expression) where T : Delegate
        {
            var constants = new List<ConstantExpression>();
            var closureCollectorVisitor = new ClosureCollectorVisitor(constants);
            expression = (Expression<T>) closureCollectorVisitor.Visit(expression);

            if (constants.Count == 0) return expression;
            
            var closureBuilder = new ClosureBuilder();
            for (var index = 0; index < constants.Count; index++)
            {
                var value = constants[index];
                closureBuilder.Add(value);
            }

            var closureDefinition = closureBuilder.Build(out var closure);
            
            var closureVisitor = new ClosureVisitor(closureDefinition);
            expression = (Expression<T>) closureVisitor.Visit(expression);

            var block = Block()
                .Parameter(closureDefinition.Key)
                .Line(Expression.Assign(closureDefinition.Key, Arg(closure)));
            
            if (expression!.Body is BlockExpression blockExpression)
            {
                var variables = blockExpression.Variables;
                for (var index = 0; index < blockExpression.Variables.Count; index++)
                {
                    block.Parameter(variables[index]);
                }

                block.Lines(blockExpression.Expressions);
            }
            else
            {
                block.Line(expression!.Body);
            }

            var lambda = block.Lambda<T>(expression.Parameters);
            return lambda;
        }

        private class ClosureVisitor : ExpressionVisitor
        {
            private readonly Dictionary<Expression, Expression> _expressions;

            public ClosureVisitor(KeyValuePair<ParameterExpression, Dictionary<Expression, Expression>> closureDefinition)
            {
                _expressions = closureDefinition.Value;
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                switch (node.Value)
                {
                    case null:
                    case string _:
                        return node;

                    default:
                        if (node.Type.GetTypeInfo().IsValueType) return node;
                        break;
                }

                if (_expressions.TryGetValue(node, out var replacement))
                {
                    if (node.Type != replacement.Type)
                    {
                        return Expression.Convert(replacement, node.Type);
                    }
                    
                    return replacement;
                }

                return base.VisitConstant(node);
            }
        }

        private class ClosureCollectorVisitor : ExpressionVisitor
        {
            private readonly List<ConstantExpression> _expressions;
            
            public ClosureCollectorVisitor(List<ConstantExpression> expressions)
            {
                _expressions = expressions;
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                var body = Visit(node.Body);
                var expression = body;
                if (expression == null) throw new InvalidOperationException("Cannot create closure");

                return node.Update(expression, node.Parameters);
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                switch (node.Value)
                {
                    case null:
                    case string _:
                        return node;

                    default:
                        if (node.Type.GetTypeInfo().IsValueType) return node;
                        break;
                }
                
                _expressions.Add(node);
                
                return node;
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
    }
}