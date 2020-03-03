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
            var configuration = context.Configuration;
            
            var writerParameter = ExpressionShortcuts.Parameter<TextWriter>("buffer");
            var objectParameter = ExpressionShortcuts.Parameter<object>("data");
            
            var bindingContext = ExpressionShortcuts.Arg<BindingContext>(context.BindingContext);
            var inlinePartialsParameter = ExpressionShortcuts.Null<IDictionary<string, Action<TextWriter, object>>>();
            var encodedWriterExpression = ExpressionShortcuts.Call(() => EncodedTextWriter.From(ExpressionShortcuts.Arg<TextWriter>(writerParameter), context.Configuration.TextEncoder));
            var newBindingContext = ExpressionShortcuts.New(
                () => new BindingContext(configuration, objectParameter, encodedWriterExpression, ExpressionShortcuts.Arg<BindingContext>(parentContext), templatePath, (IDictionary<string, Action<TextWriter, object>>) inlinePartialsParameter)
            );

            var blockBuilder = ExpressionShortcuts.Block()
                .Parameter(bindingContext)
                .Line(bindingContext.TernaryAssign(objectParameter.Is<BindingContext>(), objectParameter.As<BindingContext>(), newBindingContext))
                .Lines(((BlockExpression) body).Expressions);
            
            return Expression.Lambda<Action<TextWriter, object>>(blockBuilder, (ParameterExpression) writerParameter.Expression, (ParameterExpression) objectParameter.Expression);
        }
    }
}