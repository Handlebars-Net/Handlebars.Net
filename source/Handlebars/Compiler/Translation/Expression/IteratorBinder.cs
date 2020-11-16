using System.Linq.Expressions;
using Expressions.Shortcuts;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.ObjectDescriptors;
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
            var context = CompilationContext.Args.BindingContext;
            var writer = CompilationContext.Args.EncodedWriter;

            var template = FunctionBuilder.Compile(new[] {iex.Template}, new CompilationContext(CompilationContext));
            var ifEmpty = FunctionBuilder.Compile(new[] {iex.IfEmpty}, new CompilationContext(CompilationContext));

            if (iex.Sequence is PathExpression pathExpression)
            {
                pathExpression.Context = PathExpression.ResolutionContext.Parameter;
            }
            
            var compiledSequence = Arg<object>(FunctionBuilder.Reduce(iex.Sequence, CompilationContext));
            var blockParamsValues = CreateBlockParams();

            return Call(() =>
                Iterator.Iterate(context, writer, blockParamsValues, compiledSequence, template, ifEmpty)
            );
            
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

            if (!ObjectDescriptor.TryCreate(target, context.Configuration.ObjectDescriptorProvider, out var descriptor))
            {
                throw new HandlebarsRuntimeException($"Cannot create ObjectDescriptor for type {descriptor.DescribedType}");
            }

            if (descriptor.Iterator == null) throw new HandlebarsRuntimeException($"Type {descriptor.DescribedType} does not support iteration");
            
            descriptor.Iterator.Iterate(writer, context, blockParamsVariables, target, template, ifEmpty);
        }
    }
}

