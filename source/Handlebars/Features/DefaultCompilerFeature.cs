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

    [FeatureOrder(2)]
    internal class DefaultCompilerFeature : IFeature
    {
        public void OnCompiling(ICompiledHandlebarsConfiguration configuration)
        {
            var templateFeature = configuration.Features
                .OfType<ClosureFeature>()
                .SingleOrDefault();
            
            configuration.ExpressionCompiler = new DefaultExpressionCompiler(configuration, templateFeature);
        }

        public void CompilationCompleted()
        {
            // noting to do here
        }

        private class DefaultExpressionCompiler : IExpressionCompiler
        {
            private readonly ClosureFeature _closureFeature;
            private readonly ICollection<IExpressionMiddleware> _expressionMiddleware;
            private readonly Dictionary<Expression, object> _alreadyCompiledExpressions;

            public DefaultExpressionCompiler(ICompiledHandlebarsConfiguration configuration, ClosureFeature closureFeature)
            {
                _closureFeature = closureFeature;
                _expressionMiddleware = configuration.ExpressionMiddleware;
                _alreadyCompiledExpressions = new Dictionary<Expression, object>();
            }
            
            public T Compile<T>(Expression<T> expression) where T: class
            {
                var originalExpression = expression;
                if (_alreadyCompiledExpressions.TryGetValue(expression, out var compiled))
                {
                    return (T) compiled;
                }
                
                expression = (Expression<T>) _expressionMiddleware.Aggregate((Expression) expression, (e, m) => m.Invoke(e));
                
                var closureFeature = _closureFeature;
                
                if (closureFeature.TemplateClosure.CurrentIndex == -1)
                {
                    closureFeature = new ClosureFeature();
                    _closureFeature.Children.AddLast(closureFeature);
                }
                
                var templateClosure = closureFeature.TemplateClosure;
                var closure = closureFeature.ClosureInternal;

                expression = (Expression<T>) closureFeature.ExpressionMiddleware.Invoke(expression);

                var parameters = new[] { (ParameterExpression) closure }.Concat(expression.Parameters);
                var lambda = Expression.Lambda(expression.Body, parameters);
                var compiledLambda = lambda.Compile();
                
                var outerParameters = expression.Parameters
                    .Select(o => Expression.Parameter(o.IsByRef ? o.Type.MakeByRefType() : o.Type, o.Name))
                    .ToArray();
                
                var store = Arg(templateClosure).Member(o => o.Store);
                var parameterExpressions = new[] { store.Expression }.Concat(outerParameters);
                var invocationExpression = Expression.Invoke(Expression.Constant(compiledLambda), parameterExpressions);
                var outerLambda = Expression.Lambda<T>(invocationExpression, outerParameters);

                var compiledExpression = outerLambda.Compile();
                
                _alreadyCompiledExpressions.Add(originalExpression, compiledExpression);
                
                return compiledExpression;
            }
        }
    }
}