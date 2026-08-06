using System.Text.Json;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.Runtime;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet.Iterators
{
    public sealed class JsonElementIterator : IIterator
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
            var element = (JsonElement) input;
            switch (element.ValueKind)
            {
                case JsonValueKind.Array:
                    IterateArray(element, writer, context, blockParamsVariables, template, ifEmpty);
                    break;
                case JsonValueKind.Object:
                    IterateObject(element, writer, context, blockParamsVariables, template, ifEmpty);
                    break;
                default:
                    using (var innerContext = context.CreateFrame())
                    {
                        innerContext.Value = context.Value;
                        ifEmpty(writer, innerContext);
                    }
                    break;
            }
        }

        private static void IterateArray(
            JsonElement target,
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

            var index = 0;
            var lastIndex = target.GetArrayLength() - 1;
            foreach (var value in target.EnumerateArray())
            {
                var indexObject = BoxedValues.Int(index);

                if (index == 1) iterator.First = BoxedValues.False;
                if (index == lastIndex) iterator.Last = BoxedValues.True;

                iterator.Key = iterator.Index = indexObject;

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

        private static void IterateObject(
            JsonElement target,
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

            var count = 0;
            foreach (var _ in target.EnumerateObject()) count++;

            var index = 0;
            var lastIndex = count - 1;
            foreach (var property in target.EnumerateObject())
            {
                if (index == 1) iterator.First = BoxedValues.False;
                if (index == lastIndex) iterator.Last = BoxedValues.True;

                var value = property.Value;
                var key = property.Name;

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
