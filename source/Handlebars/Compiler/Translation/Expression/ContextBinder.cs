using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class ContextBinder : HandlebarsExpressionVisitor
    {
        private ContextBinder()
            : base(null)
        {
        }

        public static Expression<Action<TextWriter, object>> Bind(Expression body, CompilationContext context, Expression parentContext, string templatePath)
        {
            var writerParameter = E.Parameter<TextWriter>("buffer");
            var objectParameter = E.Parameter<object>("data");
            
            var bindingContext = E.Arg<BindingContext>(context.BindingContext);
            var inlinePartialsParameter = E.Null<IDictionary<string, Action<TextWriter, object>>>();
            var encodedWriterExpression = E.Call(() => EncodedTextWriter.From(E.Arg<TextWriter>(writerParameter), context.Configuration.TextEncoder));
            var newBindingContext = E.New(
                () => new BindingContext(objectParameter, encodedWriterExpression, E.Arg<BindingContext>(parentContext), templatePath, (IDictionary<string, Action<TextWriter, object>>) inlinePartialsParameter)
            );

            var blockBuilder = E.Block().Parameter(bindingContext)
                .Line(bindingContext.TernaryAssign(objectParameter.Is<BindingContext>(), objectParameter.As<BindingContext>(), newBindingContext))
                .Lines(((BlockExpression) body).Expressions);
            
            return Expression.Lambda<Action<TextWriter, object>>(blockBuilder, (ParameterExpression) writerParameter.Expression, (ParameterExpression) objectParameter.Expression);
        }
    }
}