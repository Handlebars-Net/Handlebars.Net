using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers
{
    public sealed class DelegateReturnHelperDescriptor : ReturnHelperDescriptor
    {
        private readonly HandlebarsReturnHelper _helper;

        public DelegateReturnHelperDescriptor(PathInfo name, HandlebarsReturnHelper helper) : base(name)
        {
            _helper = helper;
        }
        
        public DelegateReturnHelperDescriptor(string name, HandlebarsReturnHelper helper) : base(name)
        {
            _helper = helper;
        }

        public override object Invoke(object context, params object[] arguments)
        {
            return _helper(context, arguments);
        }
    }
}