using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Helpers
{
    public sealed class LookupReturnHelperDescriptor : IHelperDescriptor<HelperOptions>
    {
        public PathInfo Name => "lookup";

        public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
        {
            if (arguments.Length != 2 && arguments.Length != 3)
            {
                throw new HandlebarsException("{{lookup}} helper must have two or three arguments");
            }

            var segment = ChainSegment.Create(arguments[1]);

            var defaultValueIfNotFound = arguments.Length == 3 ? arguments[2] : UndefinedBindingResult.Create(segment);

            return options.TryAccessMember(arguments[0], segment, out var value)
                ? value
                : defaultValueIfNotFound;
        }

        public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
        {
            output.Write(Invoke(options, context, arguments));
        }
    }
}