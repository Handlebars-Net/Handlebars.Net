using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.ObjectDescriptors;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.Runtime;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet.Iterators
{
    public sealed class DynamicObjectIterator : IIterator
    {
        private readonly ObjectDescriptor _descriptor;

        public DynamicObjectIterator(ObjectDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        public void Iterate(
            in EncodedTextWriter writer, 
            BindingContext context,
            ChainSegment[] blockParamsVariables,
            object input,
            TemplateDelegate template,
            TemplateDelegate ifEmpty
        )
        {
            using var innerContext = context.CreateFrame();
            var iterator = new IteratorValues(innerContext);
            var blockParamsValues = new BlockParamsValues(innerContext, blockParamsVariables);
            
            blockParamsValues.CreateProperty(0, out var _0);
            blockParamsValues.CreateProperty(1, out var _1);
            
            var properties = _descriptor.GetProperties(_descriptor, input).Cast<ChainSegment>();
            var enumerator = ExtendedEnumerator<ChainSegment>.Create(properties.GetEnumerator());

            iterator.First = BoxedValues.True;
            iterator.Last = BoxedValues.False;

            int index = 0;
            var accessor = new ObjectAccessor(input, _descriptor);
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                
                var iteratorKey = current.Value;
                iterator.Key = iteratorKey;
                
                if (index == 1) iterator.First = BoxedValues.False;
                if (current.IsLast) iterator.Last = BoxedValues.True;
                
                iterator.Index = BoxedValues.Int(index);
                
                var resolvedValue = accessor[iteratorKey];
                
                blockParamsValues[_0] = resolvedValue;
                blockParamsValues[_1] = iteratorKey;
                
                iterator.Value = resolvedValue;
                innerContext.Value = resolvedValue;

                template(writer, innerContext);

                ++index;
            }
            
            if (index == 0)
            {
                innerContext.Value = context.Value;
                ifEmpty(writer, innerContext);
            }
        }
    }
}