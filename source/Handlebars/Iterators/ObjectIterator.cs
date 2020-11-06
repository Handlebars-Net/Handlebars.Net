using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.ObjectDescriptors;
using HandlebarsDotNet.Runtime;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet.Iterators
{
    public sealed class ObjectIterator : IIterator
    {
        private readonly ObjectDescriptor _descriptor;

        public ObjectIterator(ObjectDescriptor descriptor)
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
            var iterator = new ObjectIteratorValues(innerContext);
            var blockParamsValues = new BlockParamsValues(innerContext, blockParamsVariables);
            
            blockParamsValues.CreateProperty(0, out var _0);
            blockParamsValues.CreateProperty(1, out var _1);
            
            var properties = (ChainSegment[]) _descriptor.GetProperties(_descriptor, input);
            
            iterator.First = BoxedValues.True;
            iterator.Last = BoxedValues.False;

            var index = 0;
            var lastIndex = properties.Length - 1;
            var accessor = new ObjectAccessor(input, _descriptor);
            for(; index < properties.Length; index++)
            {
                var iteratorKey = properties[index];
                iterator.Key = iteratorKey;
                
                if (index == 1) iterator.First = BoxedValues.False;
                if (index == lastIndex) iterator.Last = BoxedValues.True;
                
                iterator.Index = BoxedValues.Int(index);
                
                var resolvedValue = accessor[iteratorKey];
                
                blockParamsValues[_0] = resolvedValue;
                blockParamsValues[_1] = iteratorKey;
                
                iterator.Value = resolvedValue;
                innerContext.Value = resolvedValue;

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