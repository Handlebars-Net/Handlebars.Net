using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    public sealed class WithBlockHelperDescriptor : IHelperDescriptor<BlockHelperOptions>
    {
        public PathInfo Name { get; } = "with";
    
        public object Invoke(in BlockHelperOptions options, in Context context, in Arguments arguments)
        {
            return this.ReturnInvoke(options, context, arguments);
        }

        public void Invoke(in EncodedTextWriter output, in BlockHelperOptions options, in Context context, in Arguments arguments)
        {
            if (arguments.Length != 1)
            {
                throw new HandlebarsException("{{with}} helper must have exactly one argument");
            }

            if (!HandlebarsUtils.IsTruthyOrNonEmpty(arguments[0]))
            {
                options.Inverse(output, context);
                return;
            }

            using var frame = options.CreateFrame(arguments[0]);
            var blockParamsValues = new BlockParamsValues(frame, options.BlockVariables);
            blockParamsValues[0] = arguments[0];

            options.Template(output, frame);
        }
    }
}