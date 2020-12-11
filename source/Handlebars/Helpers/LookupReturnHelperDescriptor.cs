using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Helpers
{
    public sealed class LookupReturnHelperDescriptor : IHelperDescriptor<HelperOptions>
    {
        public PathInfo Name { get; } = "lookup";

        public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
        {
            if (arguments.Length != 2)
            {
                throw new HandlebarsException("{{lookup}} helper must have exactly two argument");
            }
            
            var segment = ChainSegment.Create(arguments[1]);
            return !options.TryAccessMember(arguments[0], segment, out var value)
                ? UndefinedBindingResult.Create(segment)
                : value;
        }

        public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
        {
            output.Write(Invoke(options, context, arguments));
        }
    }
}