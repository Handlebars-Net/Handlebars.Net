using System;
using System.Linq;
using System.Linq.Expressions;
using System.IO;

namespace Handlebars.Compiler
{
    internal class FunctionWrapper
    {
        public static Expression<Action<TextWriter, object>> Wrap(Expression body, Expression parentContext)
        {
            var writerParameter = Expression.Parameter(typeof(TextWriter));
            var objectParameter = Expression.Parameter(typeof(object));
            return Expression.Lambda<Action<TextWriter, object>>(
                CreateFunctionBody(body, writerParameter, objectParameter, parentContext),
                new[] { writerParameter, objectParameter });
        }

        private static Expression CreateFunctionBody(
            Expression body,
            ParameterExpression writerParameter,
            ParameterExpression objectParameter,
            Expression parentContext)
        {
            if (parentContext == null)
            {
                parentContext = Expression.Constant(null, typeof(BindingContext));
            }
            var bindingContext = Expression.New(
                 typeof(BindingContext).GetConstructor(
                     new [] { typeof(object), typeof(TextWriter), typeof(BindingContext) }),
                new Expression[] { objectParameter, writerParameter, parentContext });

            return Expression.Invoke(body, new Expression[] { bindingContext });
        }
    }
}

