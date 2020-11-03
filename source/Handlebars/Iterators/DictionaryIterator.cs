using System.Collections;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.Runtime;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet.Iterators
{
    public sealed class DictionaryIterator<TDictionary> : IIterator
        where TDictionary : class, IDictionary
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
            var properties = target.Keys;
            var enumerator = properties.GetEnumerator();

            iterator.First = BoxedValues.True;
            iterator.Last = BoxedValues.False;

            var index = 0;
            var lastIndex = properties.Count - 1;
            while (enumerator.MoveNext())
            {
                var iteratorKey = enumerator.Current;
                iterator.Key = iteratorKey;
                
                if (index == 1) iterator.First = BoxedValues.False;
                if (index == lastIndex) iterator.Last = BoxedValues.True;
                
                iterator.Index = BoxedValues.Int(index);
                
                var resolvedValue = target[iteratorKey!];
                
                blockParamsValues[_0] = resolvedValue;
                blockParamsValues[_1] = iteratorKey;
                
                iterator.Value = resolvedValue;
                innerContext.Value = resolvedValue;

                template(writer, innerContext);

                index++;
            }

            if (index == 0)
            {
                innerContext.Value = context.Value;
                ifEmpty(writer, innerContext);
            }
        }
    }
}