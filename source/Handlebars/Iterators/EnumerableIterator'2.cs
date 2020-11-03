using System.Collections.Generic;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.Runtime;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet.Iterators
{
    public sealed class EnumerableIterator<T, TValue> : IIterator 
        where T : class, IEnumerable<TValue>
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
            var outerEnumerator = target.GetEnumerator();
            var enumerator = new ExtendedEnumerator<TValue, IEnumerator<TValue>>(outerEnumerator);

            iterator.First = BoxedValues.True;
            iterator.Last = BoxedValues.False;

            int index = 0;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                
                var value = (object) current.Value;
                var indexObject = BoxedValues.Int(index);
                
                if (index == 1) iterator.First = BoxedValues.False;
                if (current.IsLast) iterator.Last = BoxedValues.True;
                
                iterator.Index = indexObject;
                
                blockParamsValues[_0] = value;
                blockParamsValues[_1] = indexObject;
                
                iterator.Value = value;
                innerContext.Value = value;

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