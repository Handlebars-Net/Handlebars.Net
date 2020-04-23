using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Expressions.Shortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal class ContextBinder : HandlebarsExpressionVisitor
    {
        private ContextBinder()
        {
        }

        public static Expression<Action<TextWriter, object>> Bind(CompilationContext context, Expression body, Expression parentContext, string templatePath)
        {
            var configuration = ExpressionShortcuts.Arg(context.Configuration);
            
            var writerParameter = ExpressionShortcuts.Parameter<TextWriter>("buffer");
            var objectParameter = ExpressionShortcuts.Parameter<object>("data");
            
            var bindingContext = ExpressionShortcuts.Arg<BindingContext>(context.BindingContext);
            var inlinePartialsParameter = ExpressionShortcuts.Null<IDictionary<string, Action<TextWriter, object>>>();
            var textEncoder = configuration.Property(o => o.TextEncoder);
            var encodedWriterExpression = ExpressionShortcuts.Call(() => EncodedTextWriter.From(writerParameter, (ITextEncoder) textEncoder));
            var parentContextArg = ExpressionShortcuts.Arg<BindingContext>(parentContext);
            
            var newBindingContext = ExpressionShortcuts.Call(
                () => BindingContext.Create(configuration, objectParameter, encodedWriterExpression, parentContextArg, templatePath, (IDictionary<string, Action<TextWriter, object>>) inlinePartialsParameter)
            );

            var shouldDispose = ExpressionShortcuts.Var<bool>("shouldDispose");

            Expression blockBuilder = ExpressionShortcuts.Block()
                .Parameter(bindingContext)
                .Parameter(shouldDispose)
                .Line(ExpressionShortcuts.Condition()
                    .If(objectParameter.Is<BindingContext>(),
                        bindingContext.Assign(objectParameter.As<BindingContext>())
                    )
                    .Else(block =>
                    {
                        block.Line(shouldDispose.Assign(true));
                        block.Line(bindingContext.Assign(newBindingContext));
                    })
                )
                .Line(ExpressionShortcuts.Try()
                    .Body(block => block.Lines(((BlockExpression) body).Expressions))
                    .Finally(ExpressionShortcuts.Condition()
                        .If(shouldDispose, bindingContext.Call(o => o.Dispose()))
                    )
                );

            return Expression.Lambda<Action<TextWriter, object>>(blockBuilder, (ParameterExpression) writerParameter.Expression, (ParameterExpression) objectParameter.Expression);
        }
        
        public static Expression<Action<BindingContext, TextWriter, object>> Bind(CompilationContext context, Expression body, string templatePath)
        {
            var configuration = ExpressionShortcuts.Arg(context.Configuration);
            
            var writerParameter = ExpressionShortcuts.Parameter<TextWriter>("buffer");
            var objectParameter = ExpressionShortcuts.Parameter<object>("data");
            
            var bindingContext = ExpressionShortcuts.Arg<BindingContext>(context.BindingContext);
            var inlinePartialsParameter = ExpressionShortcuts.Null<IDictionary<string, Action<TextWriter, object>>>();
            var textEncoder = configuration.Property(o => o.TextEncoder);
            var encodedWriterExpression = ExpressionShortcuts.Call(() => EncodedTextWriter.From(writerParameter, (ITextEncoder) textEncoder));
            var parentContextArg = ExpressionShortcuts.Var<BindingContext>("parentContext");
            
            var newBindingContext = ExpressionShortcuts.Call(
                () => BindingContext.Create(configuration, objectParameter, encodedWriterExpression, parentContextArg, templatePath, (IDictionary<string, Action<TextWriter, object>>) inlinePartialsParameter)
            );
            
            var shouldDispose = ExpressionShortcuts.Var<bool>("shouldDispose");

            Expression blockBuilder = ExpressionShortcuts.Block()
                .Parameter(bindingContext)
                .Parameter(shouldDispose)
                .Line(ExpressionShortcuts.Condition()
                    .If(objectParameter.Is<BindingContext>(),
                        bindingContext.Assign(objectParameter.As<BindingContext>())
                    )
                    .Else(block =>
                    {
                        block.Line(shouldDispose.Assign(true));
                        block.Line(bindingContext.Assign(newBindingContext));
                    })
                )
                .Line(ExpressionShortcuts.Try()
                    .Body(block => block.Lines(((BlockExpression) body).Expressions))
                    .Finally(ExpressionShortcuts.Condition()
                        .If(shouldDispose, bindingContext.Call(o => o.Dispose()))
                    )
                );
            
            return Expression.Lambda<Action<BindingContext, TextWriter, object>>(blockBuilder, (ParameterExpression) parentContextArg.Expression, (ParameterExpression) writerParameter.Expression, (ParameterExpression) objectParameter.Expression);
        }
    }
}