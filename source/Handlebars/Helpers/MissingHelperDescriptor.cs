using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Helpers
{
    public sealed class MissingHelperDescriptor : IHelperDescriptor<HelperOptions>
    {
        public PathInfo Name { get; } = "helperMissing";
        
        public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
        {
            if (arguments.Length > 0)
            {
                throw new HandlebarsRuntimeException($"Template references a helper that cannot be resolved. Helper '{options.Name}'");
            }
            
            return UndefinedBindingResult.Create(options.Name);
        }

        public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
        {
            output.Write(Invoke(options, context, arguments));
        }
    }
}