using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Expressions.Shortcuts;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet.Compiler
{
    internal class BlockHelperFunctionBinder : HandlebarsExpressionVisitor
    {
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
            
            var context = ExpressionShortcuts.Arg<BindingContext>(CompilationContext.BindingContext);
            var bindingContext = isInlinePartial 
                ? context.Cast<object>()
                : context.Property(o => o.Value);

            var readerContext = ExpressionShortcuts.Arg(bhex.Context);
            var body = FunctionBuilder.CompileCore(((BlockExpression) bhex.Body).Expressions, CompilationContext.Configuration);
            var inverse = FunctionBuilder.CompileCore(((BlockExpression) bhex.Inversion).Expressions, CompilationContext.Configuration);
            var helperName = bhex.HelperName.TrimStart('#', '^');
            var textWriter = context.Property(o => o.TextWriter);
            var arguments = ExpressionShortcuts.Array<object>(bhex.Arguments.Select(o => FunctionBuilder.Reduce(o, CompilationContext)));
            var configuration = ExpressionShortcuts.Arg(CompilationContext.Configuration);
            
            var reducerNew = ExpressionShortcuts.New(() => new LambdaReducer(context, body, inverse));
            var reducer = ExpressionShortcuts.Var<LambdaReducer>();

            var blockParamsProvider = ExpressionShortcuts.Var<BlockParamsValueProvider>();
            var blockParamsExpression = ExpressionShortcuts.Call(
                () => BlockParamsValueProvider.Create(context, ExpressionShortcuts.Arg<BlockParam>(bhex.BlockParams))
            );
            
            var helperOptions = ExpressionShortcuts.New(
                () => new HelperOptions(
                    reducer.Property(o => o.Direct), 
                    reducer.Property(o => o.Inverse), 
                    blockParamsProvider,
                    configuration)
            );

            var blockHelpers = CompilationContext.Configuration.BlockHelpers;
            if (blockHelpers.TryGetValue(helperName, out var helper))
            {
                return ExpressionShortcuts.Block()
                    .Parameter(reducer, reducerNew)
                    .Parameter(blockParamsProvider, blockParamsExpression)
                    .Line(blockParamsProvider.Using((self, builder) =>
                    {
                        builder
                            .Line(context.Call(o => o.RegisterValueProvider((IValueProvider) self)))
                            .Line(ExpressionShortcuts.Try()
                                .Body(ExpressionShortcuts.Call(
                                    () => helper(textWriter, helperOptions, bindingContext, arguments)
                                ))
                                .Finally(context.Call(o => o.UnregisterValueProvider((IValueProvider) self)))
                            );
                    }));
            }

            foreach (var resolver in CompilationContext.Configuration.HelperResolvers)
            {
                if (resolver.TryResolveBlockHelper(helperName, out helper))
                    return ExpressionShortcuts.Block()
                        .Parameter(reducer, reducerNew)
                        .Parameter(blockParamsProvider, blockParamsExpression)
                        .Line(blockParamsProvider.Using((self, builder) =>
                        {
                            builder
                                .Line(context.Call(o => o.RegisterValueProvider((IValueProvider) self)))
                                .Line(ExpressionShortcuts.Try()
                                    .Body(ExpressionShortcuts.Call(
                                        () => helper(textWriter, helperOptions, bindingContext, arguments)
                                    ))
                                    .Finally(context.Call(o => o.UnregisterValueProvider((IValueProvider) self)))
                                );
                        }));
            }
            
            var helperPrefix = bhex.HelperName[0];
            return ExpressionShortcuts.Block()
                .Parameter(reducer, reducerNew)
                .Parameter(blockParamsProvider, blockParamsExpression)
                .Line(blockParamsProvider.Using((self, builder) =>
                {
                    builder
                        .Line(context.Call(o => o.RegisterValueProvider((IValueProvider) self)))
                        .Line(ExpressionShortcuts.Try()
                            .Body(ExpressionShortcuts.Call(
                                () => LateBoundCall(
                                    helperName, 
                                    helperPrefix, 
                                    context, 
                                    (IReaderContext) readerContext, 
                                    textWriter, helperOptions,
                                    body,
                                    inverse,
                                    bindingContext,
                                    self,
                                    arguments
                                )
                            ))
                            .Finally(context.Call(o => o.UnregisterValueProvider((IValueProvider) self)))
                        );
                }));
        }
        
        private static void LateBoundCall(
            string helperName, 
            char helperPrefix, 
            BindingContext bindingContext, 
            IReaderContext readerContext,
            TextWriter output, 
            HelperOptions options, 
            Action<BindingContext, TextWriter,object> body, 
            Action<BindingContext, TextWriter,object> inverse,
            dynamic context, 
            BlockParamsValueProvider blockParamsValueProvider,
            params object[] arguments
        )
        {
            try
            {
                if (bindingContext.Configuration.BlockHelpers.TryGetValue(helperName, out var helper))
                {
                    helper(output, options, context, arguments);
                    return;
                }
                
                foreach (var resolver in bindingContext.Configuration.HelperResolvers)
                {
                    if (!resolver.TryResolveBlockHelper(helperName, out helper)) continue;
                    
                    helper(output, options, context, arguments);
                        
                    return;
                }
                
                bindingContext.TryGetContextVariable(ref ChainSegment.Create(helperName), out var value);
                DeferredSectionBlockHelper.Helper(bindingContext, helperPrefix, value, body, inverse, blockParamsValueProvider);
            }
            catch(Exception e)
            {
                throw new HandlebarsRuntimeException($"Error occured while executing `{helperName}.`", e, readerContext);
            }
        }
    }
}

