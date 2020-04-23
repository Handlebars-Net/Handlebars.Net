using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Expressions.Shortcuts;

namespace HandlebarsDotNet.Features
{
    internal class DefaultCompilerFeatureFactory : IFeatureFactory
    {
        public IFeature CreateFeature()
        {
            return new DefaultCompilerFeature();
        }
    }

    internal class DefaultCompilerFeature : IFeature
    {
        public void OnCompiling(HandlebarsConfiguration configuration)
        {
            var templateFeature = ((InternalHandlebarsConfiguration) configuration).Features
                .OfType<ClosureFeature>().SingleOrDefault();
            
            configuration.CompileTimeConfiguration.ExpressionCompiler = new DefaultExpressionCompiler(configuration, templateFeature);
        }

        public void CompilationCompleted()
        {
        }

        private class DefaultExpressionCompiler : IExpressionCompiler
        {
            private readonly ClosureFeature _closureFeature;
            private readonly TemplateClosure _templateClosure;
            private readonly ExpressionContainer<object[]> _closure;
            private readonly ICollection<IExpressionMiddleware> _expressionMiddleware;

            public DefaultExpressionCompiler(HandlebarsConfiguration configuration, ClosureFeature closureFeature)
            {
                _closureFeature = closureFeature;
                _templateClosure = closureFeature?.TemplateClosure;
                _closure = closureFeature?.Closure;
                _expressionMiddleware = configuration.CompileTimeConfiguration.ExpressionMiddleware;
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

                var store = ExpressionShortcuts.Arg(_templateClosure).Property(o => o.Store);
                var outerLambda = Expression.Lambda<T>(
                    Expression.Invoke(Expression.Constant(compiledLambda), new[] {store.Expression}.Concat(outerParameters)),
                    outerParameters);

                return outerLambda.Compile();
            }
        }
    }
}