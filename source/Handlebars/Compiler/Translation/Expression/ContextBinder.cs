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
            var textEncoder = configuration.Property(o => o.TextEncoder);
            var encodedWriterExpression = Call(() => EncodedTextWriter.From(writerParameter, (ITextEncoder) textEncoder));
            var parentContextArg = Arg<BindingContext>(parentContext);
            
            var newBindingContext = Call(
                () => BindingContext.Create((ICompiledHandlebarsConfiguration) configuration, objectParameter, encodedWriterExpression, parentContextArg, templatePath)
            );
            
            Expression blockBuilder = Block()
                .Parameter(bindingContext)
                .Parameter<bool>(out var shouldDispose)
                .Line(bindingContext.Assign(objectParameter.As<BindingContext>()))
                .Line(Condition()
                    .If(Expression.Equal(bindingContext, Null<BindingContext>()))
                    .Then(block => 
                    {
                        block.Line(shouldDispose.Assign(true));
                        block.Line(bindingContext.Assign(newBindingContext));
                    })
                )
                .Line(Try()
                    .Body(block => block.Lines(((BlockExpression) body).Expressions))
                    .Finally(Condition()
                        .If(shouldDispose)
                        .Then(block =>
                        {
                            block
                                .Line(bindingContext.Call(o => o.TextWriter.Dispose()))
                                .Line(bindingContext.Call(o => o.Dispose()));
                        })
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
            var textEncoder = configuration.Property(o => o.TextEncoder);
            var writer = Var<EncodedTextWriter>("writer");
            var encodedWriterExpression = Call(() => EncodedTextWriter.From(writerParameter, (ITextEncoder) textEncoder));
            var parentContextArg = Var<BindingContext>("parentContext");
            
            var newBindingContext = Call(
                () => BindingContext.Create((ICompiledHandlebarsConfiguration) configuration, objectParameter, writer, parentContextArg, templatePath)
            );
            
            Expression blockBuilder = Block()
                .Parameter(bindingContext)
                .Parameter(writer)
                .Parameter<bool>(out var shouldDispose)
                .Parameter<bool>(out var shouldDisposeWriter)
                .Line(bindingContext.Assign(objectParameter.As<BindingContext>()))
                .Line(Condition()
                    .If(Expression.Equal(bindingContext, Null<BindingContext>()))
                    .Then(block =>
                    {
                        block.Line(shouldDispose.Assign(true))
                            .Line(writer.Assign(writerParameter.As<EncodedTextWriter>()))
                            .Line(Condition()
                                .If(Expression.Equal(writer, Null<EncodedTextWriter>()))
                                .Then(b =>
                                {
                                    b.Line(shouldDisposeWriter.Assign(true))
                                     .Line(writer.Assign(encodedWriterExpression));
                                })
                            );
                        
                        block.Line(bindingContext.Assign(newBindingContext));
                    })
                )
                .Line(Try()
                    .Body(block => block.Lines(((BlockExpression) body).Expressions))
                    .Finally(block =>
                    {
                        block.Lines(
                            Condition()
                                .If(shouldDispose)
                                .Then(bindingContext.Call(o => o.Dispose())),
                            Condition()
                                .If(shouldDisposeWriter)
                                .Then(writer.Call(o => o.Dispose()))
                        );
                    })
                );
            
            return Expression.Lambda<Action<BindingContext, TextWriter, object>>(blockBuilder, (ParameterExpression) parentContextArg.Expression, (ParameterExpression) writerParameter.Expression, (ParameterExpression) objectParameter.Expression);
        }
    }
}