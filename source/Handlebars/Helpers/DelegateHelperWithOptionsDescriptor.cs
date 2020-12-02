using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Helpers
{
    public sealed class DelegateHelperWithOptionsDescriptor : IHelperDescriptor<HelperOptions>
    {
        private readonly HandlebarsHelperWithOptions _helper;

        public DelegateHelperWithOptionsDescriptor(string name, HandlebarsHelperWithOptions helper)
        {
            _helper = helper;
            Name = name;
        }

        public PathInfo Name { get; }
        
        public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
        {
            return this.ReturnInvoke(options, context, arguments);
        }

        public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
        {
            _helper(output, options, context, arguments);
        }
    }
}