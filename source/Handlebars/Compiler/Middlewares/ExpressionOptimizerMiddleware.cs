using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HandlebarsDotNet.Pools;

namespace HandlebarsDotNet.Compiler.Middlewares
{
    internal class ExpressionOptimizerMiddleware : IExpressionMiddleware
    {
        public Expression<T> Invoke<T>(Expression<T> expression) where T : Delegate
        {
            using var container = GenericObjectPool<OptimizationVisitor>.Shared.Use();
            using var visitor = container.Value;
            return (Expression<T>) visitor.Visit(expression);
        }
        
        private class OptimizationVisitor : ExpressionVisitor, IDisposable
        {
            private readonly Dictionary<object, ConstantExpression> _constantExpressions = new();
            
            protected override Expression VisitBlock(BlockExpression node)
            {
                if (node.Variables.Count == 0 && node.Expressions.Count == 1 && node.Expressions[0] is BlockExpression blockExpression)
                {
                    return VisitBlock(blockExpression);
                }
                
                return base.VisitBlock(node);
            }
            
            protected override Expression VisitUnary(UnaryExpression node)
            {
                switch (node.NodeType)
                {
                    case ExpressionType.Convert:
                        if (node.Operand.Type == node.Type)
                        {
                            return node.Operand;
                        }
                        break;
                }
                    
                return base.VisitUnary(node);
            }
            
            protected override Expression VisitConstant(ConstantExpression node)
            {
                if(node.Value != null && _constantExpressions.TryGetValue(node.Value, out var storedNode))
                {
                    return storedNode;
                }

                if (node.Value != null)
                {
                    _constantExpressions.Add(node.Value, node);
                }
                    
                return node;
            }

            public void Dispose() => _constantExpressions.Clear();
        }
    }
}