using System.Linq.Expressions;

namespace HandlebarsDotNet.Features
{
    internal class ExpressionOptimizerFeatureFactory : IFeatureFactory
    {
        public IFeature CreateFeature() => new ExpressionOptimizerFeature();
    }

    internal class ExpressionOptimizerFeature : IFeature, IExpressionMiddleware
    {
        private static readonly EmptyBlockExpressionLifter EmptyBlockLifter = new EmptyBlockExpressionLifter();
        
        public void OnCompiling(ICompiledHandlebarsConfiguration configuration)
        {
            configuration.ExpressionMiddleware.Add(this);
        }

        public void CompilationCompleted()
        {
            // nothing to do here
        }
        
        private class EmptyBlockExpressionLifter : ExpressionVisitor
        {
            protected override Expression VisitBlock(BlockExpression node)
            {
                if (node.Variables.Count == 0 && node.Expressions.Count == 1 && node.Expressions[0] is BlockExpression blockExpression)
                {
                    return blockExpression;
                }
                
                return base.VisitBlock(node);
            }
        }

        public Expression Invoke(Expression expression)
        {
            expression = EmptyBlockLifter.Visit(expression);
            
            return expression;
        }
    }
}