using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    public sealed class DelegateReturnBlockHelperDescriptor : IHelperDescriptor<BlockHelperOptions>
    {
        private readonly HandlebarsReturnBlockHelper _helper;

        public DelegateReturnBlockHelperDescriptor(string name, HandlebarsReturnBlockHelper helper)
        {
            _helper = helper;
            Name = name;
        }
        
        public PathInfo Name { get; }

        public object Invoke(in BlockHelperOptions options, in Context context, in Arguments arguments)
        {
            return _helper(options, context, arguments);
        }

        public void Invoke(in EncodedTextWriter output, in BlockHelperOptions options, in Context context, in Arguments arguments)
        {
            output.Write(_helper(options, context, arguments));
        }
    }
}