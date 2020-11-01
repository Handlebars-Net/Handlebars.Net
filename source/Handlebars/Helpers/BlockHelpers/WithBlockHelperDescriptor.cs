using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    internal sealed class WithBlockHelperDescriptor : BlockHelperDescriptor
    {
        public WithBlockHelperDescriptor() : base("with")
        {
        }

        public override void Invoke(in EncodedTextWriter output, in BlockHelperOptions options, object context, in Arguments arguments)
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