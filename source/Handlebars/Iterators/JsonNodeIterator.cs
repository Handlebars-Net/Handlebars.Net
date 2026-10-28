using System.Text.Json.Nodes;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.Runtime;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet.Iterators
{
    public sealed class JsonNodeIterator : IIterator
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
            if (input is JsonObject obj)
            {
                IterateObject(obj, writer, context, blockParamsVariables, template, ifEmpty);
                return;
            }

            using var innerContext = context.CreateFrame();
            innerContext.Value = context.Value;
            ifEmpty(writer, innerContext);
        }

        private static void IterateObject(
            JsonObject target,
            in EncodedTextWriter writer,
            BindingContext context,
            ChainSegment[] blockParamsVariables,
            TemplateDelegate template,
            TemplateDelegate ifEmpty
        )
        {
            using var innerContext = context.CreateFrame();
            var iterator = new IteratorValues(innerContext);
            var blockParamsValues = new BlockParamsValues(innerContext, blockParamsVariables);

            blockParamsValues.CreateProperty(0, out var _0);
            blockParamsValues.CreateProperty(1, out var _1);

            iterator.First = BoxedValues.True;
            iterator.Last = BoxedValues.False;

            var count = target.Count;

            var index = 0;
            var lastIndex = count - 1;
            foreach (var property in target)
            {
                if (index == 1)
                {
                    iterator.First = BoxedValues.False;
                }
                if (index == lastIndex)
                {
                    iterator.Last = BoxedValues.True;
                }

                var value = property.Value;
                var key = property.Key;

                iterator.Key = key;
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
