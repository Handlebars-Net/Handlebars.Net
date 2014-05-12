using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace Handlebars.Compiler
{
    internal class ContextBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression body, CompilationContext context, Expression parentContext)
        {
            var writerParameter = Expression.Parameter(typeof(TextWriter));
            var objectParameter = Expression.Parameter(typeof(object));
            if (parentContext == null)
            {
                parentContext = Expression.Constant(null, typeof(BindingContext));
            }
            var bindingContext = Expression.New(
                 typeof(BindingContext).GetConstructor(
                     new[] { typeof(object), typeof(TextWriter), typeof(BindingContext) }),
                new Expression[] { objectParameter, writerParameter, parentContext });
            return Expression.Lambda<Action<TextWriter, object>>(
                Expression.Block(
                    new [] { context.BindingContext },
                    new Expression[] {
                        Expression.Assign(context.BindingContext, bindingContext)
                    }.Concat(
                        ((BlockExpression)body).Expressions
                    )),
                new[] { writerParameter, objectParameter });
        }
        
    }
}

