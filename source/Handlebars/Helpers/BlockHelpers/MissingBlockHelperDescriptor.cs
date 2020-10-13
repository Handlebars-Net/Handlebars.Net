using System.IO;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    internal sealed class MissingBlockHelperDescriptor : BlockHelperDescriptor
    {
        public MissingBlockHelperDescriptor() : base("missingBlockHelper")
        {
        }

        public override void Invoke(TextWriter output, HelperOptions options, object context, params object[] arguments)
        {
            var pathInfo = options.GetValue<PathInfo>("path");
            if(arguments.Length > 0) throw new HandlebarsRuntimeException($"Template references a helper that cannot be resolved. BlockHelper '{pathInfo}'");
            
            var bindingContext = options.BindingContext;
            var value = PathResolver.ResolvePath(bindingContext, pathInfo);
            DeferredSectionBlockHelper.PlainHelper(bindingContext, value, options.OriginalTemplate, options.OriginalInverse);
        }
    }
}