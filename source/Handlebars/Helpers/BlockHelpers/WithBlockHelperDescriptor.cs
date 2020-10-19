using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    internal sealed class WithBlockHelperDescriptor : BlockHelperDescriptor
    {
        public WithBlockHelperDescriptor(ICompiledHandlebarsConfiguration configuration) : base(configuration.PathInfoStore.GetOrAdd("with"))
        {
        }

        public override void Invoke(in EncodedTextWriter output, in HelperOptions options, object context, in Arguments arguments)
        {
            if (arguments.Length != 1)
            {
                throw new HandlebarsException("{{with}} helper must have exactly one argument");
            }
            
            if (HandlebarsUtils.IsTruthyOrNonEmpty(arguments[0]))
            {
                using var frame = options.CreateFrame(arguments[0]);
                var blockParamsValues = new BlockParamsValues(frame, options.BlockParams);
                
                blockParamsValues.CreateProperty(0, out var _0);

                blockParamsValues[_0] = arguments[0];
                
                options.Template(output, frame);
            }
            else
            {
                options.Inverse(output, context);
            }
        }
    }
}