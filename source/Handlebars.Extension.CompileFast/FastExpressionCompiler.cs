using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Expressions.Shortcuts;
using FastExpressionCompiler;
using HandlebarsDotNet.Features;

namespace HandlebarsDotNet.Extension.CompileFast
{
    internal class FastExpressionCompiler : IExpressionCompiler
    {
        private readonly ClosureFeature _closureFeature;
        private readonly TemplateClosure _templateClosure;
        private readonly ExpressionContainer<object[]> _closure;
        private readonly ICollection<IExpressionMiddleware> _expressionMiddleware;

        public FastExpressionCompiler(HandlebarsConfiguration configuration, ClosureFeature closureFeature)
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
                return expression.CompileFast();
            }
            
            expression = (Expression<T>) _closureFeature.ExpressionMiddleware.Invoke(expression);

            var parameters = new[] { (ParameterExpression) _closure }.Concat(expression.Parameters).ToArray();
            var lambda = Expression.Lambda(expression.Body, parameters);
            var compiledDelegateType = Expression.GetDelegateType(parameters.Select(o => o.Type).Concat(new[] {lambda.ReturnType}).ToArray());
            
            var method = typeof(FastExpressionCompiler)
                .GetMethod(nameof(CompileGeneric), BindingFlags.Static | BindingFlags.NonPublic)
                ?.MakeGenericMethod(compiledDelegateType);
            
            var compiledLambda = method?.Invoke(null, new object[] { lambda }) ?? throw new InvalidOperationException("lambda cannot be compiled");

            var outerParameters = expression.Parameters.Select(o => Expression.Parameter(o.Type, o.Name)).ToArray();

            var store = ExpressionShortcuts.Arg(_templateClosure).Property(o => o.Store);
            var outerLambda = Expression.Lambda<T>(
                Expression.Invoke(Expression.Constant(compiledLambda), new[] {store.Expression}.Concat(outerParameters)),
                outerParameters);
            
            return outerLambda.CompileFast();
        }

        private static T CompileGeneric<T>(LambdaExpression expression) where T : class
        {
            return expression.CompileFast<T>();
        }
    }
}