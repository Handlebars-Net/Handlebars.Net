using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler.Middlewares
{
    internal class ExpressionOptimizerMiddleware : IExpressionMiddleware
    {
        public Expression<T> Invoke<T>(Expression<T> expression) where T : Delegate
        {
            var visitor = new OptimizationVisitor();
            return (Expression<T>) visitor.Visit(expression);
        }
        
        private class OptimizationVisitor : ExpressionVisitor
        {
            private readonly Dictionary<object, ConstantExpression> _constantExpressions = new Dictionary<object, ConstantExpression>();
            
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
        }
    }
}