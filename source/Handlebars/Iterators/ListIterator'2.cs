using System.Collections.Generic;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.Runtime;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet.Iterators
{
    public class ListIterator<T, TValue> : IIterator
        where T: IList<TValue>
    {
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

            var target = (T) input;
            var count = target.Count;

            iterator.First = BoxedValues.True;
            iterator.Last = BoxedValues.False;
            
            var index = 0;
            var lastIndex = count - 1;
            for (; index < count; index++)
            {
                var value = (object) target[index];
                var objectIndex = BoxedValues.Int(index);
                
                if (index == 1) iterator.First = BoxedValues.False;
                if (index == lastIndex) iterator.Last = BoxedValues.True;

                iterator.Key = iterator.Index = objectIndex;
                
                blockParamsValues[_0] = value;
                blockParamsValues[_1] = objectIndex;
                
                iterator.Value = value;
                innerContext.Value = value;

                template(writer, innerContext);
            }

            if (index == 0)
            {
                innerContext.Value = context.Value;
                ifEmpty(writer, innerContext);
            }
        }
    }
}