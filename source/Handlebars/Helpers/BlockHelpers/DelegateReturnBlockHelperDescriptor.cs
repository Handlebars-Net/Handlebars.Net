using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    public sealed class DelegateReturnBlockHelperDescriptor : ReturnBlockHelperDescriptor
    {
        private readonly HandlebarsReturnBlockHelper _helper;

        public DelegateReturnBlockHelperDescriptor(string name, HandlebarsReturnBlockHelper helper) : base(name)
        {
            _helper = helper;
        }
        
        public DelegateReturnBlockHelperDescriptor(PathInfo name, HandlebarsReturnBlockHelper helper) : base(name)
        {
            _helper = helper;
        }

        public override object Invoke(in HelperOptions options, object context, in Arguments arguments)
        {
            return _helper(options, context, arguments);
        }
    }
}