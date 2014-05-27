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
            var newBindingContext = Expression.New(
                 typeof(BindingContext).GetConstructor(
                     new[] { typeof(object), typeof(TextWriter), typeof(BindingContext) }),
                new Expression[] { objectParameter, Expression.Call(new Func<TextWriter, TextWriter>(GetEncodedWriter).Method,writerParameter), parentContext });
            return Expression.Lambda<Action<TextWriter, object>>(
                Expression.Block(
                    new [] { context.BindingContext },
                    new Expression[] {
                        Expression.IfThenElse(
                            Expression.TypeIs(objectParameter, typeof(BindingContext)),
                            Expression.Assign(context.BindingContext, Expression.TypeAs(objectParameter, typeof(BindingContext))),
                            Expression.Assign(context.BindingContext, newBindingContext))
                    }.Concat(
                        ((BlockExpression)body).Expressions
                    )),
                new[] { writerParameter, objectParameter });
        }

        private static TextWriter GetEncodedWriter(TextWriter writer)
        {
            if(typeof(EncodedTextWriter).IsAssignableFrom(writer.GetType()))
            {
                return writer;
            }
            else
            {
                return new EncodedTextWriter(writer);
            }
        }
    }
}

