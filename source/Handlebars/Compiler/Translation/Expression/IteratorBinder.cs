using System;
using System.Linq.Expressions;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Expressions.Shortcuts;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.MemberAccessors;
using HandlebarsDotNet.ObjectDescriptors;
using HandlebarsDotNet.Polyfills;
using HandlebarsDotNet.ValueProviders;
using static Expressions.Shortcuts.ExpressionShortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal static class BoxedValues
    {
        public static readonly object True = true;
        public static readonly object False = false;
        public static readonly object Zero = 0;
    }
    
    internal class IteratorBinder : HandlebarsExpressionVisitor
    {
        private CompilationContext CompilationContext { get; }
        
        public IteratorBinder(CompilationContext compilationContext)
        {
            CompilationContext = compilationContext;
        }
        
        protected override Expression VisitIteratorExpression(IteratorExpression iex)
        {
            var context = Arg<BindingContext>(CompilationContext.BindingContext);

            var template = FunctionBuilder.CompileCore(new[] {iex.Template}, CompilationContext.Configuration);
            var ifEmpty = FunctionBuilder.CompileCore(new[] {iex.IfEmpty}, CompilationContext.Configuration);

            if (iex.Sequence is PathExpression pathExpression)
            {
                pathExpression.Context = PathExpression.ResolutionContext.Parameter;
            }
            
            var compiledSequence = Arg<object>(FunctionBuilder.Reduce(iex.Sequence, CompilationContext));
            var blockParamsValues = CreateBlockParams();

            return Call(() =>
                Iterator.Iterate(context, blockParamsValues, compiledSequence, template, ifEmpty)
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
        public static void Iterate(BindingContext context,
            ChainSegment[] blockParamsVariables,
            object target,
            Action<BindingContext, TextWriter, object> template,
            Action<BindingContext, TextWriter, object> ifEmpty)
        {
            if (!HandlebarsUtils.IsTruthy(target))
            {
                ifEmpty(context, context.TextWriter, context.Value);
                return;
            }

            var targetType = target.GetType();
            if (!context.Configuration.ObjectDescriptorProvider.TryGetDescriptor(targetType, out var descriptor))
            {
                ifEmpty(context, context.TextWriter, context.Value);
                return;
            }

            if (!descriptor.ShouldEnumerate)
            {
                var properties = descriptor.GetProperties(descriptor, target);
                if (properties is IList<ChainSegment> propertiesList)
                {
                    IterateObjectWithStaticProperties(context, blockParamsVariables, descriptor, target, propertiesList, targetType, template, ifEmpty);
                    return;   
                }
                
                IterateObject(context, descriptor, blockParamsVariables, target, properties, targetType, template, ifEmpty);
                return;
            }

            if (target is IList list)
            {
                IterateList(context, blockParamsVariables, list, template, ifEmpty);
                return;
            }

            IterateEnumerable(context, blockParamsVariables, (IEnumerable) target, template, ifEmpty);
        }
        
        private static void IterateObject(BindingContext context,
            ObjectDescriptor descriptor,
            ChainSegment[] blockParamsVariables,
            object target,
            IEnumerable properties,
            Type targetType,
            Action<BindingContext, TextWriter, object> template,
            Action<BindingContext, TextWriter, object> ifEmpty)
        {
            using var innerContext = context.CreateChildContext();
            var iterator = new ObjectIteratorValues(innerContext);
            var blockParams = new BlockParamsValues(innerContext, blockParamsVariables);
            
            blockParams.CreateProperty(0, out var _0);
            blockParams.CreateProperty(1, out var _1);
            
            var accessor = new MemberAccessor(target, descriptor);
            var enumerable = new ExtendedEnumerable<object>(properties);
            var enumerated = false;

            object iteratorValue;
            ChainSegment iteratorKey;
            
            foreach (var enumerableValue in enumerable)
            {
                enumerated = true;
                iteratorKey = ChainSegment.Create(enumerableValue.Value);
                
                iterator.Key = iteratorKey;
                iterator.Index = enumerableValue.Index;
                if (enumerableValue.Index == 1) iterator.First = BoxedValues.False;
                if (enumerableValue.IsLast) iterator.Last = BoxedValues.True;

                iteratorValue = accessor[iteratorKey];
                iterator.Value = iteratorValue;
                innerContext.Value = iteratorValue;
                        
                blockParams[_0] = iteratorValue;
                blockParams[_1] = iteratorKey;
                
                template(context, context.TextWriter, innerContext);
            }

            if (!enumerated)
            {
                ifEmpty(context, context.TextWriter, context.Value);
            }
        }

        private static void IterateObjectWithStaticProperties(BindingContext context,
            ChainSegment[] blockParamsVariables,
            ObjectDescriptor descriptor,
            object target,
            IList<ChainSegment> properties,
            Type targetType,
            Action<BindingContext, TextWriter, object> template,
            Action<BindingContext, TextWriter, object> ifEmpty)
        {
            using var innerContext = context.CreateFrame();
            var iterator = new ObjectIteratorValues(innerContext);
            var blockParams = new BlockParamsValues(innerContext, blockParamsVariables);
            
            blockParams.CreateProperty(0, out var _0);
            blockParams.CreateProperty(1, out var _1);
            
            var count = properties.Count;
            var accessor = new MemberAccessor(target, descriptor);

            var iterationIndex = 0;
            var lastIndex = count - 1;
            
            object iteratorValue;
            ChainSegment iteratorKey;

            for (; iterationIndex < count; iterationIndex++)
            {
                iteratorKey = properties[iterationIndex];

                iterator.Index = iterationIndex;
                iterator.Key = iteratorKey;
                if (iterationIndex == 1) iterator.First = BoxedValues.False;
                if (iterationIndex == lastIndex) iterator.Last = BoxedValues.True;

                iteratorValue = accessor[iteratorKey];
                iterator.Value = iteratorValue;
                innerContext.Value = iteratorValue;
                
                blockParams[_0] = iteratorValue;
                blockParams[_1] = iteratorKey;

                template(context, context.TextWriter, innerContext);
            }

            if (iterationIndex == 0)
            {
                ifEmpty(context, context.TextWriter, context.Value);
            }
        }
        
        private static void IterateList(BindingContext context,
            ChainSegment[] blockParamsVariables,
            IList target,
            Action<BindingContext, TextWriter, object> template,
            Action<BindingContext, TextWriter, object> ifEmpty)
        {
            using var innerContext = context.CreateFrame();
            var iterator = new IteratorValues(innerContext);
            var blockParams = new BlockParamsValues(innerContext, blockParamsVariables);
            
            blockParams.CreateProperty(0, out var _0);
            blockParams.CreateProperty(1, out var _1);
            
            var count = target.Count;

            object boxedIndex;
            object iteratorValue;
            
            var iterationIndex = 0;
            var lastIndex = count - 1;
            for (; iterationIndex < count; iterationIndex++)
            {
                iteratorValue = target[iterationIndex];
                
                iterator.Value = iteratorValue;
                if (iterationIndex == 1) iterator.First = BoxedValues.False;
                if (iterationIndex == lastIndex) iterator.Last = BoxedValues.True;
                
                boxedIndex = iterationIndex;
                iterator.Index = boxedIndex;
                
                blockParams[_0] = iteratorValue;
                blockParams[_1] = boxedIndex;
                
                innerContext.Value = iteratorValue;
                
                template(context, context.TextWriter, innerContext);
            }

            if (iterationIndex == 0)
            {
                ifEmpty(context, context.TextWriter, context.Value);
            }
        }
        
        private static void IterateEnumerable(BindingContext context,
            ChainSegment[] blockParamsVariables,
            IEnumerable target,
            Action<BindingContext, TextWriter, object> template,
            Action<BindingContext, TextWriter, object> ifEmpty)
        {
            using var innerContext = context.CreateChildContext();
            var iterator = new IteratorValues(innerContext);
            var blockParams = new BlockParamsValues(innerContext, blockParamsVariables);

            blockParams.CreateProperty(0, out var _0);
            blockParams.CreateProperty(1, out var _1);
            
            var enumerable = new ExtendedEnumerable<object>(target);
            var enumerated = false;

            object boxedIndex;            
            object iteratorValue;
            
            foreach (var enumerableValue in enumerable)
            {
                enumerated = true;
                
                if (enumerableValue.Index == 1) iterator.First = BoxedValues.False;
                if (enumerableValue.IsLast) iterator.Last = BoxedValues.True;

                boxedIndex = enumerableValue.Index;
                iteratorValue = enumerableValue.Value;
                iterator.Value = iteratorValue;
                iterator.Index = boxedIndex;
                
                blockParams[_0] = iteratorValue;
                blockParams[_1] = boxedIndex;
                
                innerContext.Value = iteratorValue;
                
                template(context, context.TextWriter, innerContext);
            }

            if (!enumerated)
            {
                ifEmpty(context, context.TextWriter, context.Value);
            }
        }
    }
}

