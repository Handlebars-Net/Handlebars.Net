using System;
using System.Linq;
using System.Linq.Expressions;
using System.IO;

namespace Handlebars.Compiler
{
    internal class FunctionWrapper
    {
        public static Expression<Action<TextWriter, object>> Wrap(Expression body)
        {
            var writerParameter = Expression.Parameter(typeof(TextWriter));
            var objectParameter = Expression.Parameter(typeof(object));
            return Expression.Lambda<Action<TextWriter, object>>(
                CreateFunctionBody(body, writerParameter, objectParameter),
                new[] { writerParameter, objectParameter });
        }

        private static Expression CreateFunctionBody(
            Expression body,
            ParameterExpression writerParameter,
            ParameterExpression objectParameter)
        {
            var bindingContext = Expression.New(
                 typeof(BindingContext).GetConstructor(
                     new [] { typeof(object), typeof(TextWriter), typeof(BindingContext) }),
                new Expression[] { objectParameter, writerParameter, Expression.Constant(null, typeof(BindingContext)) });

            return Expression.Invoke(body, new Expression[] { bindingContext });
        }
    }
}

