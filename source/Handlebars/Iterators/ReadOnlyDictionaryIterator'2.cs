using System.Collections.Generic;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.Runtime;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet.Iterators
{
    public sealed class ReadOnlyDictionaryIterator<TDictionary, TKey, TValue> : IIterator
        where TDictionary : class, IReadOnlyDictionary<TKey, TValue>
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
            var iterator = new ObjectIteratorValues(innerContext);
            var blockParamsValues = new BlockParamsValues(innerContext, blockParamsVariables);
            
            blockParamsValues.CreateProperty(0, out var _0);
            blockParamsValues.CreateProperty(1, out var _1);

            var target = (TDictionary) input;
            using var enumerator = target.GetEnumerator();

            iterator.First = BoxedValues.True;
            iterator.Last = BoxedValues.False;

            var index = 0;
            int lastIndex = target.Count - 1;
            while (enumerator.MoveNext())
            {
                var key = (object) enumerator.Current.Key;
                var value = (object) enumerator.Current.Value;
                
                iterator.Key = key;
                
                if (index == 1) iterator.First = BoxedValues.False;
                if (index == lastIndex) iterator.Last = BoxedValues.True;
                
                iterator.Index = BoxedValues.Int(index);

                blockParamsValues[_0] = value;
                blockParamsValues[_1] = key;
                
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