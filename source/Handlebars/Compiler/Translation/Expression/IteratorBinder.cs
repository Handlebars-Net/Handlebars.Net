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
            var context = CompilationContext.Args.BindingContext;
            var writer = CompilationContext.Args.EncodedWriter;

            var template = FunctionBuilder.Compile(new[] {iex.Template}, CompilationContext.Configuration);
            var ifEmpty = FunctionBuilder.Compile(new[] {iex.IfEmpty}, CompilationContext.Configuration);

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

            var targetType = target.GetType();
            if (!context.Configuration.ObjectDescriptorProvider.TryGetDescriptor(targetType, out var descriptor))
            {
                using var frame = context.CreateFrame(context.Value);
                ifEmpty(writer, frame);
                return;
            }

            if (!descriptor.ShouldEnumerate)
            {
                var properties = descriptor.GetProperties(descriptor, target);
                if (properties is IList<ChainSegment> propertiesList)
                {
                    IterateObjectWithStaticProperties(context, writer, blockParamsVariables, descriptor, target, propertiesList, targetType, template, ifEmpty);
                    return;   
                }
                
                IterateObject(context, writer, descriptor, blockParamsVariables, target, properties, targetType, template, ifEmpty);
                return;
            }

            if (target is IList list)
            {
                IterateList(context, writer, blockParamsVariables, list, template, ifEmpty);
                return;
            }

            IterateEnumerable(context, writer, blockParamsVariables, (IEnumerable) target, template, ifEmpty);
        }
        
        private static void IterateObject(
            BindingContext context,
            EncodedTextWriter writer,
            ObjectDescriptor descriptor,
            ChainSegment[] blockParamsVariables,
            object target,
            IEnumerable properties,
            Type targetType,
            TemplateDelegate template,
            TemplateDelegate ifEmpty)
        {
            using var innerContext = context.CreateFrame();
            var iterator = new ObjectIteratorValues(innerContext);
            var blockParams = new BlockParamsValues(innerContext, blockParamsVariables);
            
            blockParams.CreateProperty(0, out var _0);
            blockParams.CreateProperty(1, out var _1);
            
            var accessor = new MemberAccessor(target, descriptor);
            var enumerable = new ExtendedEnumerator<object>(properties.GetEnumerator());
            var enumerated = false;

            object iteratorValue;
            ChainSegment iteratorKey;

            while (enumerable.MoveNext())
            {
                enumerated = true;
                var enumerableValue = enumerable.Current;
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
                
                template(writer, innerContext);
            }

            if (!enumerated)
            {
                innerContext.Value = context.Value;
                ifEmpty(writer, innerContext);
            }
        }

        private static void IterateObjectWithStaticProperties(
            BindingContext context,
            EncodedTextWriter writer,
            ChainSegment[] blockParamsVariables,
            ObjectDescriptor descriptor,
            object target,
            IList<ChainSegment> properties,
            Type targetType,
            TemplateDelegate template,
            TemplateDelegate ifEmpty)
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

                template(writer, innerContext);
            }

            if (iterationIndex == 0)
            {
                innerContext.Value = context.Value;
                ifEmpty(writer, innerContext);
            }
        }
        
        private static void IterateList(
            BindingContext context,
            EncodedTextWriter writer,
            ChainSegment[] blockParamsVariables,
            IList target,
            TemplateDelegate template,
            TemplateDelegate ifEmpty)
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
                
                template(writer, innerContext);
            }

            if (iterationIndex == 0)
            {
                innerContext.Value = context.Value;
                ifEmpty(writer, innerContext);
            }
        }
        
        private static void IterateEnumerable(
            BindingContext context,
            EncodedTextWriter writer,
            ChainSegment[] blockParamsVariables,
            IEnumerable target,
            TemplateDelegate template,
            TemplateDelegate ifEmpty)
        {
            using var innerContext = context.CreateFrame();
            var iterator = new IteratorValues(innerContext);
            var blockParams = new BlockParamsValues(innerContext, blockParamsVariables);

            blockParams.CreateProperty(0, out var _0);
            blockParams.CreateProperty(1, out var _1);
            
            var enumerator = new ExtendedEnumerator<object>(target.GetEnumerator());
            var enumerated = false;

            object boxedIndex;            
            object iteratorValue;

            while (enumerator.MoveNext())
            {
                enumerated = true;
                var enumerableValue = enumerator.Current;
                
                if (enumerableValue.Index == 1) iterator.First = BoxedValues.False;
                if (enumerableValue.IsLast) iterator.Last = BoxedValues.True;

                boxedIndex = enumerableValue.Index;
                iteratorValue = enumerableValue.Value;
                iterator.Value = iteratorValue;
                iterator.Index = boxedIndex;
                
                blockParams[_0] = iteratorValue;
                blockParams[_1] = boxedIndex;
                
                innerContext.Value = iteratorValue;
                
                template(writer, innerContext);
            }

            if (!enumerated)
            {
                innerContext.Value = context.Value;
                ifEmpty(writer, innerContext);
            }
        }
    }
}

