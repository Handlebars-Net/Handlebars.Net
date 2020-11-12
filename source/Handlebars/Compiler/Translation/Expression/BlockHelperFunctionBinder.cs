using System.Linq.Expressions;
using Expressions.Shortcuts;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.BlockHelpers;
using HandlebarsDotNet.Polyfills;
using HandlebarsDotNet.Runtime;
using static Expressions.Shortcuts.ExpressionShortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal class BlockHelperFunctionBinder : HandlebarsExpressionVisitor
    {
        private enum BlockHelperDirection { Direct, Inverse }

        private CompilationContext CompilationContext { get; }

        public BlockHelperFunctionBinder(CompilationContext compilationContext)
        {
            CompilationContext = compilationContext;
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            return sex.Body is BlockHelperExpression ? Visit(sex.Body) : sex;
        }

        protected override Expression VisitBlockHelperExpression(BlockHelperExpression bhex)
        {
            var isInlinePartial = bhex.HelperName == "#*inline";
            
            var pathInfo = PathInfoStore.Shared.GetOrAdd(bhex.HelperName);
            var bindingContext = CompilationContext.Args.BindingContext;
            var context = isInlinePartial
                ? bindingContext.As<object>()
                : bindingContext.Property(o => o.Value);
            
            var readerContext = bhex.Context;
            var direct = Compile(bhex.Body);
            var inverse = Compile(bhex.Inversion);
            var args = FunctionBinderHelpers.CreateArguments(bhex.Arguments, CompilationContext);
            
            var helperName = pathInfo.TrimmedPath;
            var direction = bhex.IsRaw || pathInfo.IsBlockHelper ? BlockHelperDirection.Direct : BlockHelperDirection.Inverse;
            var blockParams = CreateBlockParams();

            var blockHelpers = CompilationContext.Configuration.BlockHelpers;

            if (blockHelpers.TryGetValue(pathInfo, out var descriptor))
            {
                return BindByRef(descriptor);
            }

            var helperResolvers = CompilationContext.Configuration.HelperResolvers;
            for (var index = 0; index < helperResolvers.Count; index++)
            {
                var resolver = helperResolvers[index];
                if (!resolver.TryResolveBlockHelper(helperName, out var resolvedDescriptor)) continue;

                descriptor = new Ref<IHelperDescriptor<BlockHelperOptions>>(resolvedDescriptor);
                blockHelpers.AddOrReplace(pathInfo, descriptor);
                
                return BindByRef(descriptor);
            }
            
            var lateBindBlockHelperDescriptor = new LateBindBlockHelperDescriptor(pathInfo);
            var lateBindBlockHelperRef = new Ref<IHelperDescriptor<BlockHelperOptions>>(lateBindBlockHelperDescriptor);
            blockHelpers.AddOrReplace(pathInfo, lateBindBlockHelperRef);

            return BindByRef(lateBindBlockHelperRef);
            
            ExpressionContainer<ChainSegment[]> CreateBlockParams()
            {
                var parameters = bhex.BlockParams?.BlockParam?.Parameters;
                parameters ??= ArrayEx.Empty<ChainSegment>();

                return Arg(parameters);
            }
            
            TemplateDelegate Compile(Expression expression)
            {
                var blockExpression = (BlockExpression) expression;
                return FunctionBuilder.Compile(blockExpression.Expressions, CompilationContext.Configuration);
            }

            Expression BindByRef(Ref<IHelperDescriptor<BlockHelperOptions>> helperBox)
            {
                var writer = CompilationContext.Args.EncodedWriter;
                
                var helperOptions = direction switch
                {
                    BlockHelperDirection.Direct => New(() => new BlockHelperOptions(direct, inverse, blockParams, bindingContext)),
                    BlockHelperDirection.Inverse => New(() => new BlockHelperOptions(inverse, direct, blockParams, bindingContext)),
                    _ => throw new HandlebarsCompilerException("Helper referenced with unknown prefix", readerContext)
                };

                var callContext = New(() => new Context(bindingContext, context));
                return Call(() => helperBox.Value.Invoke(writer, helperOptions, callContext, args));
            }
        }
    }
}