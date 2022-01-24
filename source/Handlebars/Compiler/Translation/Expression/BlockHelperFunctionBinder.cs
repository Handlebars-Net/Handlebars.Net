using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Expressions.Shortcuts;
using HandlebarsDotNet.Decorators;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.BlockHelpers;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.Polyfills;
using HandlebarsDotNet.Runtime;
using static Expressions.Shortcuts.ExpressionShortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal enum BlockHelperDirection { Direct, Inverse }
    
    internal class BlockHelperFunctionBinder : HandlebarsExpressionVisitor
    {
        private readonly List<DecoratorDefinition> _decorators;
        
        private CompilationContext CompilationContext { get; }

        public BlockHelperFunctionBinder(CompilationContext compilationContext, List<DecoratorDefinition> decorators)
        {
            _decorators = decorators;
            CompilationContext = compilationContext;
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            return sex.Body is BlockHelperExpression ? Visit(sex.Body) : sex;
        }

        protected override Expression VisitBlockHelperExpression(BlockHelperExpression bhex)
        {
            var pathInfo = PathInfoStore.Current.GetOrAdd(bhex.HelperName);
            var bindingContext = CompilationContext.Args.BindingContext;

            var direction = bhex.IsRaw || pathInfo.IsBlockHelper ? BlockHelperDirection.Direct : BlockHelperDirection.Inverse;
            var isDecorator = direction switch
            {
                BlockHelperDirection.Direct => bhex.HelperName.StartsWith("#*"),
                BlockHelperDirection.Inverse => bhex.HelperName.StartsWith("^*"),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (isDecorator)
            { 
                _decorators.AddRange(VisitDecoratorBlockExpression(bhex));
                return Expression.Empty();
            }
            
            var readerContext = bhex.Context;
            var direct = Compile(bhex.Body, out var directDecorators);
            var inverse = Compile(bhex.Inversion, out var inverseDecorators);
            var args = FunctionBinderHelpers.CreateArguments(bhex.Arguments, CompilationContext);
            
            var context = bindingContext.Property(o => o.Value);
            
            var blockParams = CreateBlockParams();

            var blockHelpers = CompilationContext.Configuration.BlockHelpers;

            if (blockHelpers.TryGetValue(pathInfo, out var descriptor))
            {
                return BindByRef(pathInfo, descriptor);
            }

            var helperResolvers = CompilationContext.Configuration.HelperResolvers;
            for (var index = 0; index < helperResolvers.Count; index++)
            {
                var resolver = helperResolvers[index];
                if (!resolver.TryResolveBlockHelper(pathInfo, out var resolvedDescriptor)) continue;

                descriptor = new Ref<IHelperDescriptor<BlockHelperOptions>>(resolvedDescriptor);
                blockHelpers.AddOrReplace(pathInfo, descriptor);
                
                return BindByRef(pathInfo, descriptor);
            }
            
            var lateBindBlockHelperDescriptor = new LateBindBlockHelperDescriptor(pathInfo);
            var lateBindBlockHelperRef = new Ref<IHelperDescriptor<BlockHelperOptions>>(lateBindBlockHelperDescriptor);
            blockHelpers.AddOrReplace(pathInfo, lateBindBlockHelperRef);

            return BindByRef(pathInfo, lateBindBlockHelperRef);
            
            ExpressionContainer<ChainSegment[]> CreateBlockParams()
            {
                var parameters = bhex.BlockParams?.BlockParam?.Parameters;
                parameters ??= ArrayEx.Empty<ChainSegment>();

                return Arg(parameters);
            }
            
            TemplateDelegate Compile(Expression expression, out IReadOnlyList<DecoratorDefinition> decorators)
            {
                var blockExpression = (BlockExpression) expression;
                return FunctionBuilder.Compile(blockExpression.Expressions, CompilationContext, out decorators);
            }

            Expression BindByRef(PathInfo name, Ref<IHelperDescriptor<BlockHelperOptions>> helperBox)
            {
                switch (direction)
                {
                    case BlockHelperDirection.Direct when directDecorators.Count > 0:
                    {
                        var helperOptions = direction switch
                        {
                            BlockHelperDirection.Direct => New(() => new BlockHelperOptions(name, direct, inverse, blockParams, bindingContext)),
                            BlockHelperDirection.Inverse => New(() => new BlockHelperOptions(name, inverse, direct, blockParams, bindingContext)),
                            _ => throw new HandlebarsCompilerException("Helper referenced with unknown prefix", readerContext)
                        };
                        
                        var callContext = New(() => new Context(bindingContext, context));
                        
                        var writer = CompilationContext.Args.EncodedWriter;
                        var directDecorator = directDecorators.Compile(CompilationContext);
                        var templateDelegate = FunctionBuilder.Compile(
                            new []
                            {
                                Call(() => helperBox.Value.Invoke(writer, helperOptions, callContext, args)).Expression
                            }, 
                            CompilationContext, 
                            out _
                        );
                        
                        return Call(() => directDecorator.Invoke(writer, bindingContext, templateDelegate))
                            .Call(f => f.Invoke(writer, bindingContext));
                    }
                    case BlockHelperDirection.Inverse when inverseDecorators.Count > 0:
                    {
                        var helperOptions = direction switch
                        {
                            BlockHelperDirection.Direct => New(() => new BlockHelperOptions(name, direct, inverse, blockParams, bindingContext)),
                            BlockHelperDirection.Inverse => New(() => new BlockHelperOptions(name, inverse, direct, blockParams, bindingContext)),
                            _ => throw new HandlebarsCompilerException("Helper referenced with unknown prefix", readerContext)
                        };
                
                        var callContext = New(() => new Context(bindingContext, context));
                        
                        var writer = CompilationContext.Args.EncodedWriter;
                        var inverseDecorator = inverseDecorators.Compile(CompilationContext);
                        var templateDelegate = FunctionBuilder.Compile(
                            new []
                            {
                                Call(() => helperBox.Value.Invoke(writer, helperOptions, callContext, args)).Expression
                            }, 
                            CompilationContext, 
                            out _
                        );
                        
                        return Call(() => inverseDecorator.Invoke(writer, bindingContext, templateDelegate))
                            .Call(f => f.Invoke(writer, bindingContext));
                    }
                    default:
                    {
                        var helperOptions = direction switch
                        {
                            BlockHelperDirection.Direct => New(() => new BlockHelperOptions(name, direct, inverse, blockParams, bindingContext)),
                            BlockHelperDirection.Inverse => New(() => new BlockHelperOptions(name, inverse, direct, blockParams, bindingContext)),
                            _ => throw new HandlebarsCompilerException("Helper referenced with unknown prefix", readerContext)
                        };
                
                        var callContext = New(() => new Context(bindingContext, context));
                        var writer = CompilationContext.Args.EncodedWriter;
                        return Call(() => helperBox.Value.Invoke(writer, helperOptions, callContext, args));
                    }
                }
            }
        }

        private IEnumerable<DecoratorDefinition> VisitDecoratorBlockExpression(BlockHelperExpression bhex)
        {
            var pathInfo = PathInfoStore.Current.GetOrAdd(bhex.HelperName);
            var bindingContext = CompilationContext.Args.BindingContext;

            var direction = bhex.IsRaw || pathInfo.IsBlockHelper ? BlockHelperDirection.Direct : BlockHelperDirection.Inverse;
            if (direction == BlockHelperDirection.Inverse)
            {
                throw new HandlebarsCompilerException("^ is not supported for decorators", bhex.Context);
            }
            
            var direct = Compile(bhex.Body, out var decorators);
            for (var index = 0; index < decorators.Count; index++)
            {
                yield return decorators[index];
            }
            
            var args = FunctionBinderHelpers.CreateArguments(bhex.Arguments, CompilationContext);
            
            var context = bindingContext.Property(o => o.Value);
            var blockParams = CreateBlockParams();

            var blockDecorators = CompilationContext.Configuration.BlockDecorators;
            if (blockDecorators.TryGetValue(pathInfo, out var descriptor))
            {
                var binding = BindDecoratorByRef(pathInfo, descriptor, out var f1);
                yield return new DecoratorDefinition(binding, f1);
                yield break;
            }
            
            var emptyBlockDecorator = new EmptyBlockDecorator(pathInfo);
            var emptyBlockDecoratorRef = new Ref<IDecoratorDescriptor<BlockDecoratorOptions>>(emptyBlockDecorator);
            blockDecorators.AddOrReplace(pathInfo, emptyBlockDecoratorRef);

            var emptyBinding = BindDecoratorByRef(pathInfo, emptyBlockDecoratorRef, out var f2);
            yield return new DecoratorDefinition(emptyBinding, f2);
            
            ExpressionContainer<ChainSegment[]> CreateBlockParams()
            {
                var parameters = bhex.BlockParams?.BlockParam?.Parameters;
                parameters ??= ArrayEx.Empty<ChainSegment>();

                return Arg(parameters);
            }
            
            TemplateDelegate Compile(Expression expression, out IReadOnlyList<DecoratorDefinition> decorators)
            {
                var blockExpression = (BlockExpression) expression;
                return FunctionBuilder.Compile(blockExpression.Expressions, CompilationContext, out decorators);
            }

            Expression BindDecoratorByRef(PathInfo name, Ref<IDecoratorDescriptor<BlockDecoratorOptions>> helperBox, out ExpressionContainer<TemplateDelegate> function)
            {
                function = Parameter<TemplateDelegate>();
                var f = function;
                
                var helperOptions = New(() => new BlockDecoratorOptions(name, direct, blockParams, bindingContext));
                var callContext = New(() => new Context(bindingContext, context));
                
                return Call(() => helperBox.Value.Invoke(f, helperOptions, callContext, args));
            }
        }
    }
}