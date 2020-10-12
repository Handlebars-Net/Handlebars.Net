using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Expressions.Shortcuts;
using static Expressions.Shortcuts.ExpressionShortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal class ContextBinder : HandlebarsExpressionVisitor
    {
        private ContextBinder()
        {
        }

        public static Expression<Action<TextWriter, object>> Bind(CompilationContext context, Expression body, Expression parentContext, string templatePath)
        {
            var configuration = Arg(context.Configuration);
            
            var writerParameter = Parameter<TextWriter>("buffer");
            var objectParameter = Parameter<object>("data");
            
            var bindingContext = Arg<BindingContext>(context.BindingContext);
            var inlinePartialsParameter = Null<IDictionary<string, Action<TextWriter, object>>>();
            var textEncoder = configuration.Property(o => o.TextEncoder);
            var encodedWriterExpression = Call(() => EncodedTextWriter.From(writerParameter, (ITextEncoder) textEncoder));
            var parentContextArg = Arg<BindingContext>(parentContext);
            
            var newBindingContext = Call(
                () => BindingContext.Create(configuration, objectParameter, encodedWriterExpression, parentContextArg, templatePath, (IDictionary<string, Action<TextWriter, object>>) inlinePartialsParameter)
            );

            var shouldDispose = Var<bool>("shouldDispose");

            Expression blockBuilder = Block()
                .Parameter(bindingContext)
                .Parameter(shouldDispose)
                .Line(Condition()
                    .If(objectParameter.Is<BindingContext>())
                    .Then(bindingContext.Assign(objectParameter.As<BindingContext>()))
                    .Else(block =>
                    {
                        block.Line(shouldDispose.Assign(true));
                        block.Line(bindingContext.Assign(newBindingContext));
                    })
                )
                .Line(Try()
                    .Body(block => block.Lines(((BlockExpression) body).Expressions))
                    .Finally(Condition()
                        .If(shouldDispose)
                        .Then(bindingContext.Call(o => o.Dispose()))
                    )
                );

            return Expression.Lambda<Action<TextWriter, object>>(blockBuilder, (ParameterExpression) writerParameter.Expression, (ParameterExpression) objectParameter.Expression);
        }
        
        public static Expression<Action<BindingContext, TextWriter, object>> Bind(CompilationContext context, Expression body, string templatePath)
        {
            var configuration = Arg(context.Configuration);
            
            var writerParameter = Parameter<TextWriter>("buffer");
            var objectParameter = Parameter<object>("data");
            
            var bindingContext = Arg<BindingContext>(context.BindingContext);
            var inlinePartialsParameter = Null<IDictionary<string, Action<TextWriter, object>>>();
            var textEncoder = configuration.Property(o => o.TextEncoder);
            var encodedWriterExpression = Call(() => EncodedTextWriter.From(writerParameter, (ITextEncoder) textEncoder));
            var parentContextArg = Var<BindingContext>("parentContext");
            
            var newBindingContext = Call(
                () => BindingContext.Create(configuration, objectParameter, encodedWriterExpression, parentContextArg, templatePath, (IDictionary<string, Action<TextWriter, object>>) inlinePartialsParameter)
            );
            
            var shouldDispose = Var<bool>("shouldDispose");

            Expression blockBuilder = Block()
                .Parameter(bindingContext)
                .Parameter(shouldDispose)
                .Line(Condition()
                    .If(objectParameter.Is<BindingContext>())
                    .Then(bindingContext.Assign(objectParameter.As<BindingContext>())
                    )
                    .Else(block =>
                    {
                        block.Line(shouldDispose.Assign(true));
                        block.Line(bindingContext.Assign(newBindingContext));
                    })
                )
                .Line(Try()
                    .Body(block => block.Lines(((BlockExpression) body).Expressions))
                    .Finally(Condition()
                        .If(shouldDispose)
                        .Then(bindingContext.Call(o => o.Dispose()))
                    )
                );
            
            return Expression.Lambda<Action<BindingContext, TextWriter, object>>(blockBuilder, (ParameterExpression) parentContextArg.Expression, (ParameterExpression) writerParameter.Expression, (ParameterExpression) objectParameter.Expression);
        }
    }
}