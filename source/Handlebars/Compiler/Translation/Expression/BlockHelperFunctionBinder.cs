using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Expressions.Shortcuts;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.Helpers.BlockHelpers;
using HandlebarsDotNet.Polyfills;
using static Expressions.Shortcuts.ExpressionShortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal class BlockHelperFunctionBinder : HandlebarsExpressionVisitor
    {
        private static readonly LookupSlim<int, DeferredValue<Expression[], ConstructorInfo>> ArgumentsConstructorsMap = new LookupSlim<int, DeferredValue<Expression[], ConstructorInfo>>();
        private static readonly MethodInfo HelperInvokeMethodInfo = typeof(BlockHelperDescriptorBase).GetMethod(nameof(BlockHelperDescriptorBase.Invoke));

        private static readonly ConstructorInfo HelperOptionsCtor = typeof(HelperOptions)
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Single(o => o.GetParameters().Length > 0);

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

                descriptor = new StrongBox<BlockHelperDescriptorBase>(resolvedDescriptor);
                blockHelpers.Add(pathInfo, descriptor);
                
                return BindByRef(descriptor);
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
            
            TemplateDelegate Compile(Expression expression)
            {
                var blockExpression = (BlockExpression) expression;
                return FunctionBuilder.Compile(blockExpression.Expressions, CompilationContext.Configuration);
            }

            Expression BindByRef(StrongBox<BlockHelperDescriptorBase> helperBox)
            {
                var writer = CompilationContext.Args.EncodedWriter;
                
                var helperOptions = direction switch
                {
                    BlockHelperDirection.Direct => New(() => new HelperOptions(direct, inverse, blockParams, bindingContext)),
                    BlockHelperDirection.Inverse => New(() => new HelperOptions(inverse, direct, blockParams, bindingContext)),
                    _ => throw new HandlebarsCompilerException("Helper referenced with unknown prefix", readerContext)
                };
                
                return Call(() => helperBox.Value.Invoke(writer, helperOptions, context, args));
            }
        }
    }
}