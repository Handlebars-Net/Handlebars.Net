using System.Text.Json;
using System.Text.Json.Nodes;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.ObjectDescriptors;
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
            var node = (JsonNode) input;
            switch (node)
            {
                case JsonObject obj:
                    IterateObject(obj, writer, context, blockParamsVariables, template, ifEmpty);
                    break;
                case JsonArray array:
                    IterateArray(array, writer, context, blockParamsVariables, template, ifEmpty);
                    break;
                default:
                    if (node.GetValueKind() is JsonValueKind.Array or JsonValueKind.Object)
                    {
                        // A JsonValue wrapping a CLR collection or object (e.g. JsonValue.Create(new List<string>())):
                        // iterate the underlying value with its own iterator.
                        IterateWrappedValue(node, writer, context, blockParamsVariables, template, ifEmpty);
                    }
                    else
                    {
                        IterateEmpty(writer, context, ifEmpty);
                    }
                    break;
            }
        }

        private static void IterateWrappedValue(
            JsonNode node,
            in EncodedTextWriter writer,
            BindingContext context,
            ChainSegment[] blockParamsVariables,
            TemplateDelegate template,
            TemplateDelegate ifEmpty
        )
        {
            var value = node.GetValue<object>();
            if (ObjectDescriptor.TryCreate(value, out var descriptor) && descriptor.Iterator != null)
            {
                descriptor.Iterator.Iterate(writer, context, blockParamsVariables, value, template, ifEmpty);
                return;
            }

            IterateEmpty(writer, context, ifEmpty);
        }

        private static void IterateEmpty(in EncodedTextWriter writer, BindingContext context, TemplateDelegate ifEmpty)
        {
            using var innerContext = context.CreateFrame();
            innerContext.Value = context.Value;
            ifEmpty(writer, innerContext);
        }

        private static void IterateArray(
            JsonArray target,
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
            var lastIndex = target.Count - 1;
            foreach (var value in target)
            {
                var indexObject = BoxedValues.Int(index);

                if (index == 1)
                {
                    iterator.First = BoxedValues.False;
                }
                if (index == lastIndex)
                {
                    iterator.Last = BoxedValues.True;
                }

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
