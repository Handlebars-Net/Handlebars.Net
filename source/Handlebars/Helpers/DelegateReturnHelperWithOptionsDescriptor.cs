using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers
{
    public sealed class DelegateReturnHelperWithOptionsDescriptor : IHelperDescriptor<HelperOptions>
    {
        private readonly HandlebarsReturnWithOptionsHelper _helper;

        public DelegateReturnHelperWithOptionsDescriptor(string name, HandlebarsReturnWithOptionsHelper helper)
        {
            _helper = helper;
            Name = name;
        }

        public PathInfo Name { get; }

        public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
        {
            return _helper(options, context, arguments);
        }

        public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
        {
            output.Write(_helper(options, context, arguments));
        }
    }
}