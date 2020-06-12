using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Expressions.Shortcuts;
using HandlebarsDotNet.Adapters;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.ValueProviders;
using static Expressions.Shortcuts.ExpressionShortcuts;

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
            
            var context = Arg<BindingContext>(CompilationContext.BindingContext);
            var bindingContext = isInlinePartial 
                ? context.Cast<object>()
                : context.Property(o => o.Value);

            var readerContext = Arg(bhex.Context);
            var body = FunctionBuilder.CompileCore(((BlockExpression) bhex.Body).Expressions, CompilationContext.Configuration);
            var inverse = FunctionBuilder.CompileCore(((BlockExpression) bhex.Inversion).Expressions, CompilationContext.Configuration);
            var helperName = bhex.HelperName.TrimStart('#', '^');
            var helperPrefix = bhex.IsRaw ? '#' : bhex.HelperName[0];
            var textWriter = context.Property(o => o.TextWriter);
            var args = bhex.Arguments
                .ApplyOn((PathExpression pex) => pex.Context = PathExpression.ResolutionContext.Parameter)
                .Select(o => FunctionBuilder.Reduce(o, CompilationContext));
            
            var arguments = Array<object>(args);
            var configuration = Arg(CompilationContext.Configuration);
            
            var reducerNew = New(() => new LambdaReducer(context, body, inverse));
            var reducer = Var<LambdaReducer>();

            var blockParamsProvider = Var<BlockParamsValueProvider>();
            var blockParamsExpression = Call(
                () => BlockParamsValueProvider.Create(context, Arg<BlockParam>(bhex.BlockParams))
            );
            
            var helperOptions = CreateHelperOptions(bhex, helperPrefix, reducer, blockParamsProvider, configuration, context);

            var blockHelpers = CompilationContext.Configuration.BlockHelpers;
            if (blockHelpers.TryGetValue(helperName, out var helper))
            {
                return Block()
                    .Parameter(reducer, reducerNew)
                    .Parameter(blockParamsProvider, blockParamsExpression)
                    .Line(blockParamsProvider.Using((self, builder) =>
                    {
                        builder
                            .Line(context.Call(o => o.RegisterValueProvider((IValueProvider) self)))
                            .Line(Try()
                                .Body(Call(
                                    () => helper(textWriter, helperOptions, bindingContext, arguments)
                                ))
                                .Finally(context.Call(o => o.UnregisterValueProvider((IValueProvider) self)))
                            );
                    }));
            }

            foreach (var resolver in CompilationContext.Configuration.HelperResolvers)
            {
                if (!resolver.TryResolveBlockHelper(helperName, out helper)) continue;
                
                return Block()
                    .Parameter(reducer, reducerNew)
                    .Parameter(blockParamsProvider, blockParamsExpression)
                    .Line(blockParamsProvider.Using((self, builder) =>
                    {
                        builder
                            .Line(context.Call(o => o.RegisterValueProvider((IValueProvider) self)))
                            .Line(Try()
                                .Body(Call(
                                    () => helper(textWriter, helperOptions, bindingContext, arguments)
                                ))
                                .Finally(context.Call(o => o.UnregisterValueProvider((IValueProvider) self)))
                            );
                    }));
            }
            
            return Block()
                .Parameter(reducer, reducerNew)
                .Parameter(blockParamsProvider, blockParamsExpression)
                .Line(blockParamsProvider.Using((self, builder) =>
                {
                    builder
                        .Line(context.Call(o => o.RegisterValueProvider((IValueProvider) self)))
                        .Line(Try()
                            .Body(Call(
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

        private static ExpressionContainer<HelperOptions> CreateHelperOptions(
            BlockHelperExpression bhex, 
            char helperPrefix,
            ExpressionContainer<LambdaReducer> reducer, 
            ExpressionContainer<BlockParamsValueProvider> blockParamsProvider, 
            ExpressionContainer<InternalHandlebarsConfiguration> configuration,
            ExpressionContainer<BindingContext> context)
        {
            ExpressionContainer<HelperOptions> helperOptions;
            switch (helperPrefix)
            {
                case '#':
                    helperOptions = New(
                        () => new HelperOptions(
                            reducer.Member(o => o.Direct),
                            reducer.Member(o => o.Inverse),
                            blockParamsProvider,
                            configuration,
                            context)
                    );
                    break;

                case '^':
                    helperOptions = New(
                        () => new HelperOptions(
                            reducer.Member(o => o.Inverse),
                            reducer.Member(o => o.Direct),
                            blockParamsProvider,
                            configuration,
                            context)
                    );
                    break;

                default:
                    throw new HandlebarsCompilerException($"Helper {bhex.HelperName} referenced with unsupported prefix", bhex.Context);
            }

            return helperOptions;
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

                if(arguments.Length > 0) throw new HandlebarsRuntimeException($"Template references a helper that cannot be resolved. BlockHelper '{helperName}'", readerContext);
                
                var pathInfo = bindingContext.Configuration.Paths.GetOrAdd(helperName);
                var value = PathResolver.ResolvePath(bindingContext, ref pathInfo);
                DeferredSectionBlockHelper.Helper(bindingContext, helperPrefix, value, body, inverse, blockParamsValueProvider);
            }
            catch(Exception e) when(!(e is HandlebarsException))
            {
                throw new HandlebarsRuntimeException($"Error occured while executing `{helperName}.`", e, readerContext);
            }
        }
    }
}

