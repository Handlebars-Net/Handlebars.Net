using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class ContextBinder : HandlebarsExpressionVisitor
    {
        private ContextBinder()
            : base(null)
        {
        }

        public static Expression Bind(Expression body, CompilationContext context, Expression parentContext, string templatePath)
        {
            var writerParameter = Expression.Parameter(typeof(TextWriter), "buffer");
            var objectParameter = Expression.Parameter(typeof(object), "data");
            if (parentContext == null)
            {
                parentContext = Expression.Constant(null, typeof(BindingContext));
            }
            var templatePathExpression = Expression.Constant(templatePath, typeof(string));
            var outputEncoderExpression = Expression.Constant(context.Configuration.TextEncoder, typeof(ITextEncoder));
            var newBindingContext = Expression.New(
                                        typeof(BindingContext).GetConstructor(
                                            new[] { typeof(object), typeof(TextWriter), typeof(BindingContext), typeof(string), typeof(ITextEncoder) }),
                                        new Expression[] { objectParameter, writerParameter, parentContext, templatePathExpression, outputEncoderExpression });
            return Expression.Lambda<Action<TextWriter, object>>(
                Expression.Block(
                    new [] { context.BindingContext },
                    new Expression[]
                    {
                        Expression.IfThenElse(
                            Expression.TypeIs(objectParameter, typeof(BindingContext)),
                            Expression.Assign(context.BindingContext, Expression.TypeAs(objectParameter, typeof(BindingContext))),
                            Expression.Assign(context.BindingContext, newBindingContext))
                    }.Concat(
                        ((BlockExpression)body).Expressions
                    )),
                new[] { writerParameter, objectParameter });
        }
    }
}

