using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    public sealed class DelegateBlockHelperDescriptor : IHelperDescriptor<BlockHelperOptions>
    {
        private readonly HandlebarsBlockHelper _helper;

        public DelegateBlockHelperDescriptor(string name, HandlebarsBlockHelper helper)
        {
            _helper = helper;
            Name = name;
        }

        public PathInfo Name { get; }

        public object Invoke(in BlockHelperOptions options, in Context context, in Arguments arguments)
        {
            return this.ReturnInvoke(options, context, arguments);
        }

        public void Invoke(in EncodedTextWriter output, in BlockHelperOptions options, in Context context, in Arguments arguments)
        {
            _helper(output, options, context, arguments);
        }
    }
}