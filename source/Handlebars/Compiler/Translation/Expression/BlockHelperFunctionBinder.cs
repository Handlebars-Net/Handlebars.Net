using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Expressions.Shortcuts;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.Helpers.BlockHelpers;
using HandlebarsDotNet.Polyfills;
using HandlebarsDotNet.ValueProviders;
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
            
            var pathInfo = CompilationContext.Configuration.PathInfoStore.GetOrAdd(bhex.HelperName);
            var bindingContext = Arg<BindingContext>(CompilationContext.BindingContext);
            var context = isInlinePartial
                ? bindingContext.As<object>()
                : bindingContext.Property(o => o.Value);
            
            var readerContext = bhex.Context;
            var direct = Compile(bhex.Body);
            var inverse = Compile(bhex.Inversion);
            var arguments = CreateArguments();
            
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

                return Bind(resolvedDescriptor);
            }
            
            var lateBindBlockHelperDescriptor = new LateBindBlockHelperDescriptor(pathInfo, CompilationContext.Configuration);
            var lateBindBlockHelperRef = new StrongBox<BlockHelperDescriptorBase>(lateBindBlockHelperDescriptor);
            blockHelpers.Add(pathInfo, lateBindBlockHelperRef);

            return BindByRef(lateBindBlockHelperRef);
            
            ExpressionContainer<ChainSegment[]> CreateBlockParams()
            {
                var parameters = bhex.BlockParams?.BlockParam?.Parameters;
                if (parameters == null)
                {
                    parameters = ArrayEx.Empty<ChainSegment>();
                }

                return Arg(parameters);
            }
            
            ExpressionContainer<object[]> CreateArguments()
            {
                var args = bhex.Arguments
                    .ApplyOn((PathExpression pex) => pex.Context = PathExpression.ResolutionContext.Parameter)
                    .Select(o => FunctionBuilder.Reduce(o, CompilationContext));
            
                return Array<object>(args);
            }
            
            Action<BindingContext, TextWriter, object> Compile(Expression expression)
            {
                var blockExpression = (BlockExpression) expression;
                return FunctionBuilder.CompileCore(blockExpression.Expressions, CompilationContext.Configuration);
            }

            Expression BindByRef(StrongBox<BlockHelperDescriptorBase> value)
            {
                return direction switch
                {
                    BlockHelperDirection.Direct => Call(() =>
                        BlockHelperCallBindingByRef(bindingContext, context, blockParams, direct, inverse, arguments, value)),
                    
                    BlockHelperDirection.Inverse => Call(() =>
                        BlockHelperCallBindingByRef(bindingContext, context, blockParams, inverse, direct, arguments, value)),
                    
                    _ => throw new HandlebarsCompilerException("Helper referenced with unknown prefix", readerContext)
                };
            }
            
            Expression Bind(BlockHelperDescriptorBase value)
            {
                return direction switch
                {
                    BlockHelperDirection.Direct => Call(() =>
                        BlockHelperCallBinding(bindingContext, context, blockParams, direct, inverse, arguments, value)),
                    
                    BlockHelperDirection.Inverse => Call(() =>
                        BlockHelperCallBinding(bindingContext, context, blockParams, inverse, direct, arguments, value)),
                    
                    _ => throw new HandlebarsCompilerException("Helper referenced with unknown prefix", readerContext)
                };
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BlockHelperCallBindingByRef(
            BindingContext bindingContext, 
            object context,
            ChainSegment[] blockParamsVariables,
            Action<BindingContext, TextWriter, object> direct,
            Action<BindingContext, TextWriter, object> inverse,
            object[] arguments,
            StrongBox<BlockHelperDescriptorBase> helper)
        {
            using var helperOptions = HelperOptions.Create(direct, inverse, blockParamsVariables, bindingContext);
            helper.Value.Invoke(bindingContext.TextWriter, helperOptions, context, arguments);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BlockHelperCallBinding(
            BindingContext bindingContext, 
            object context,
            ChainSegment[] blockParamsVariables,
            Action<BindingContext, TextWriter, object> direct,
            Action<BindingContext, TextWriter, object> inverse,
            object[] arguments,
            BlockHelperDescriptorBase helper)
        {
            using var helperOptions = HelperOptions.Create(direct, inverse, blockParamsVariables, bindingContext);
            helper.Invoke(bindingContext.TextWriter, helperOptions, context, arguments);
        }
    }
}