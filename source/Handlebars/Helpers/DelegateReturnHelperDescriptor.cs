using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers
{
    public sealed class DelegateReturnHelperDescriptor : IHelperDescriptor<HelperOptions>
    {
        private readonly HandlebarsReturnHelper _helper;

        public DelegateReturnHelperDescriptor(string name, HandlebarsReturnHelper helper)
        {
            _helper = helper;
            Name = name;
        }

        public PathInfo Name { get; }

        public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
        {
            return _helper(context, arguments);
        }

        public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
        {
            output.Write(_helper(context, arguments));
        }
    }
}