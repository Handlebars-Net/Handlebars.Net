using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers
{
    public sealed class MissingHelperDescriptor : IHelperDescriptor<HelperOptions>
    {
        public PathInfo Name { get; } = "helperMissing";
        
        public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
        {
            var nameArgument = arguments[arguments.Length - 1];
            if (arguments.Length > 1)
            {
                throw new HandlebarsRuntimeException($"Template references a helper that cannot be resolved. Helper '{nameArgument}'");
            }
            
            var name = PathInfoStore.Shared.GetOrAdd(nameArgument as string ?? nameArgument.ToString());
            return UndefinedBindingResult.Create(name);
        }

        public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
        {
            output.Write(Invoke(options, context, arguments));
        }
    }
}