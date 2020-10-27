using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    internal sealed class MissingBlockHelperDescriptor : BlockHelperDescriptor
    {
        public MissingBlockHelperDescriptor() : base("missingBlockHelper")
        {
        }

        public override void Invoke(in EncodedTextWriter output, in HelperOptions options, object context, in Arguments arguments)
        {
            var pathInfo = options.GetValue<PathInfo>("path");
            if(arguments.Length > 0) throw new HandlebarsRuntimeException($"Template references a helper that cannot be resolved. BlockHelper '{pathInfo}'");
            
            var bindingContext = options.Frame;
            var value = PathResolver.ResolvePath(bindingContext, pathInfo);
            DeferredSectionBlockHelper.PlainHelper(bindingContext, output, value, options.OriginalTemplate, options.OriginalInverse);
        }
    }
}