using System;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.Runtime;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet.Iterators
{
    /// <summary>
    /// Iterates a true multi-dimensional <see cref="Array"/> (rank > 1) one row at a time,
    /// i.e. by its outer-most dimension. Each yielded value is a <see cref="MultidimensionalArraySlice"/>
    /// covering the remaining dimensions, so nested dimensions can be walked with further
    /// <c>{{#each}}</c> blocks or indexers.
    /// </summary>
    public sealed class MultidimensionalArrayIterator : IIterator
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

            var array = (Array) input;
            var count = array.GetLength(0);

            iterator.First = BoxedValues.True;
            iterator.Last = BoxedValues.False;

            var index = 0;
            var lastIndex = count - 1;
            for (; index < count; index++)
            {
                var value = (object?) new MultidimensionalArraySlice(array, new[] { index });
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
