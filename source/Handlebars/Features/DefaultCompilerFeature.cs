using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Expressions.Shortcuts;
using static Expressions.Shortcuts.ExpressionShortcuts;

namespace HandlebarsDotNet.Features
{
    internal class DefaultCompilerFeatureFactory : IFeatureFactory
    {
        public IFeature CreateFeature()
        {
            return new DefaultCompilerFeature();
        }
    }

    [FeatureOrder(1)]
    internal class DefaultCompilerFeature : IFeature
    {
        public void OnCompiling(ICompiledHandlebarsConfiguration configuration)
        {
            var templateFeature = ((InternalHandlebarsConfiguration) configuration).Features
                .OfType<ClosureFeature>().SingleOrDefault();
            
            configuration.ExpressionCompiler = new DefaultExpressionCompiler(configuration, templateFeature);
        }

        public void CompilationCompleted()
        {
            // noting to do here
        }

        private class DefaultExpressionCompiler : IExpressionCompiler
        {
            private readonly ClosureFeature _closureFeature;
            private readonly TemplateClosure _templateClosure;
            private readonly ExpressionContainer<object[]> _closure;
            private readonly ICollection<IExpressionMiddleware> _expressionMiddleware;

            public DefaultExpressionCompiler(ICompiledHandlebarsConfiguration configuration, ClosureFeature closureFeature)
            {
                _closureFeature = closureFeature;
                _templateClosure = closureFeature?.TemplateClosure;
                _closure = closureFeature?.ClosureInternal;
                _expressionMiddleware = configuration.ExpressionMiddleware;
            }

            public T Compile<T>(Expression<T> expression) where T: class
            {
                expression = (Expression<T>) _expressionMiddleware.Aggregate((Expression) expression, (e, m) => m.Invoke(e));

                if (_closureFeature == null)
                {
                    return expression.Compile();
                }

                expression = (Expression<T>) _closureFeature.ExpressionMiddleware.Invoke(expression);

                var parameters = new[] { (ParameterExpression) _closure }.Concat(expression.Parameters);
                var lambda = Expression.Lambda(expression.Body, parameters);
                var compiledLambda = lambda.Compile();
                
                var outerParameters = expression.Parameters.Select(o => Expression.Parameter(o.Type, o.Name)).ToArray();
                var store = Arg(_templateClosure).Member(o => o.Store);
                var parameterExpressions = new[] { store.Expression }.Concat(outerParameters);
                var invocationExpression = Expression.Invoke(Expression.Constant(compiledLambda), parameterExpressions);
                var outerLambda = Expression.Lambda<T>(invocationExpression, outerParameters);

                return outerLambda.Compile();
            }
        }
    }
}