using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Expressions.Shortcuts;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.Helpers.BlockHelpers;
using HandlebarsDotNet.Polyfills;
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
            var writer = Arg<EncodedTextWriter>(CompilationContext.EncodedWriter);
            
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
            
            ExpressionContainer<Arguments> CreateArguments()
            {
                var args = bhex.Arguments
                    .ApplyOn((PathExpression pex) => pex.Context = PathExpression.ResolutionContext.Parameter)
                    .Select(o => FunctionBuilder.Reduce(o, CompilationContext))
                    .ToArray();

                if (args.Length == 0)
                {
                    return Arg(Arguments.Empty);
                }
                
                var argumentTypes = new Type[args.Length];
                for (var i = 0; i < argumentTypes.Length; i++) argumentTypes[i] = typeof(object);
                var constructor = typeof(Arguments).GetConstructor(argumentTypes);
                
                if (constructor == null) // cannot handle by direct args pass
                {
                    var arr = Array<object>(args);
                    return New(() => new Arguments(arr));
                }
                
                return Arg<Arguments>(Expression.New(constructor, args));
            }
            
            TemplateDelegate Compile(Expression expression)
            {
                var blockExpression = (BlockExpression) expression;
                return FunctionBuilder.Compile(blockExpression.Expressions, CompilationContext.Configuration);
            }

            Expression BindByRef(StrongBox<BlockHelperDescriptorBase> helperBox)
            {
                var helperOptionsCtor = typeof(HelperOptions)
                    .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Single(o => o.GetParameters().Length > 0);
                
                var methodInfo = typeof(BlockHelperDescriptorBase).GetMethod(nameof(BlockHelperDescriptorBase.Invoke));

                var inst = Arg(helperBox).Member(o => o.Value);

                switch (direction)
                {
                    case BlockHelperDirection.Inverse:
                    {
                        var helperOptions = Expression.New(helperOptionsCtor, Expression.Constant(inverse), Expression.Constant(direct), blockParams, bindingContext);
                        return Expression.Call(inst, methodInfo, writer, helperOptions, context, arguments);
                    }
                    
                    case BlockHelperDirection.Direct:
                    {
                        var helperOptions = Expression.New(helperOptionsCtor, Expression.Constant(direct), Expression.Constant(inverse), blockParams, bindingContext);
                        return Expression.Call(inst, methodInfo, writer, helperOptions, context, arguments);
                    }
                    default:
                        throw new HandlebarsCompilerException("Helper referenced with unknown prefix", readerContext);
                }
            }
        }
    }
}