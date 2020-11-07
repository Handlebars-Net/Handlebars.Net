using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers
{
    public sealed class DelegateHelperDescriptor : IHelperDescriptor<HelperOptions>
    {
        private readonly HandlebarsHelper _helper;

        public DelegateHelperDescriptor(string name, HandlebarsHelper helper)
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
            _helper(output, context, arguments);
        }
    }
}