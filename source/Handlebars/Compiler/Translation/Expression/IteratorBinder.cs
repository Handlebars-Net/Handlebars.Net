using System.Linq.Expressions;
using Expressions.Shortcuts;
using HandlebarsDotNet.ObjectDescriptors;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.Polyfills;
using static Expressions.Shortcuts.ExpressionShortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal class IteratorBinder : HandlebarsExpressionVisitor
    {
        private CompilationContext CompilationContext { get; }
        
        public IteratorBinder(CompilationContext compilationContext)
        {
            CompilationContext = compilationContext;
        }
        
        protected override Expression VisitIteratorExpression(IteratorExpression iex)
        {
            var direction = iex.HelperName[0] switch
            {
                '#' => BlockHelperDirection.Direct,
                '^' => BlockHelperDirection.Inverse,
                _ => throw new HandlebarsCompilerException($"Tried to convert {iex.HelperName} expression to iterator block", iex.Context)
            };
            
            var template = FunctionBuilder.Compile(new[] {iex.Template}, CompilationContext, out var directDecorators);
            var ifEmpty = FunctionBuilder.Compile(new[] {iex.IfEmpty}, CompilationContext, out var inverseDecorators);

            if (iex.Sequence is PathExpression pathExpression)
            {
                pathExpression.Context = PathExpression.ResolutionContext.Parameter;
            }

            switch (direction)
            {
                case BlockHelperDirection.Direct when directDecorators.Count > 0:
                {
                    var context = CompilationContext.Args.BindingContext;
                    var writer = CompilationContext.Args.EncodedWriter;
                    var compiledSequence = Arg<object>(FunctionBuilder.Reduce(iex.Sequence, CompilationContext, out _));
                    var blockParamsValues = CreateBlockParams();
                    var templateDelegate = FunctionBuilder.Compile(
                        new []
                        {
                            Call(() => Iterator.Iterate(context, writer, blockParamsValues, compiledSequence, template, ifEmpty)).Expression
                        }, 
                        CompilationContext, 
                        out _
                    );

                    var decorator = directDecorators.Compile(CompilationContext);
                    return Call(() => decorator.Invoke(writer, context, templateDelegate))
                        .Call(f => f.Invoke(writer, context));
                }
                case BlockHelperDirection.Inverse when inverseDecorators.Count > 0:
                {
                    var context = CompilationContext.Args.BindingContext;
                    var writer = CompilationContext.Args.EncodedWriter;
                    var compiledSequence = Arg<object>(FunctionBuilder.Reduce(iex.Sequence, CompilationContext, out _));
                    var blockParamsValues = CreateBlockParams();
                    var templateDelegate = FunctionBuilder.Compile(
                        new []
                        {
                            Call(() => Iterator.Iterate(context, writer, blockParamsValues, compiledSequence, ifEmpty, template)).Expression
                        }, 
                        CompilationContext, 
                        out _
                    );

                    var decorator = inverseDecorators.Compile(CompilationContext);
                    return Call(() => decorator.Invoke(writer, context, templateDelegate))
                        .Call(f => f.Invoke(writer, context));
                }
                case BlockHelperDirection.Direct:
                {
                    var context = CompilationContext.Args.BindingContext;
                    var writer = CompilationContext.Args.EncodedWriter;
                    var compiledSequence = Arg<object>(FunctionBuilder.Reduce(iex.Sequence, CompilationContext, out _));
                    var blockParamsValues = CreateBlockParams();
                    return Call(() => Iterator.Iterate(context, writer, blockParamsValues, compiledSequence, template, ifEmpty));
                }
                case BlockHelperDirection.Inverse:
                {
                    var context = CompilationContext.Args.BindingContext;
                    var writer = CompilationContext.Args.EncodedWriter;
                    var compiledSequence = Arg<object>(FunctionBuilder.Reduce(iex.Sequence, CompilationContext, out _));
                    var blockParamsValues = CreateBlockParams();
                    return Call(() => Iterator.Iterate(context, writer, blockParamsValues, compiledSequence, ifEmpty, template));
                }
                default:
                {
                    throw new HandlebarsCompilerException($"Tried to convert {iex.HelperName} expression to iterator block", iex.Context);
                }
            }

            ExpressionContainer<ChainSegment[]> CreateBlockParams()
            {
                var parameters = iex.BlockParams?.BlockParam?.Parameters;
                if (parameters == null)
                {
                    parameters = ArrayEx.Empty<ChainSegment>();
                }

                return Arg(parameters);
            }
        }
    }

    internal static class Iterator
    {
        public static void Iterate(
            BindingContext context,
            EncodedTextWriter writer,
            ChainSegment[] blockParamsVariables,
            object target,
            TemplateDelegate template,
            TemplateDelegate ifEmpty)
        {
            if (!HandlebarsUtils.IsTruthy(target))
            {
                using var frame = context.CreateFrame(context.Value);
                ifEmpty(writer, frame);
                return;
            }

            if (!ObjectDescriptor.TryCreate(target, out var descriptor))
            {
                throw new HandlebarsRuntimeException($"Cannot create ObjectDescriptor for type {descriptor.DescribedType}");
            }

            if (descriptor.Iterator == null) throw new HandlebarsRuntimeException($"Type {descriptor.DescribedType} does not support iteration");
            
            descriptor.Iterator.Iterate(writer, context, blockParamsVariables, target, template, ifEmpty);
        }
    }
}

