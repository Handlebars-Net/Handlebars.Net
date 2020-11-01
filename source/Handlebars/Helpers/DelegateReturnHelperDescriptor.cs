namespace HandlebarsDotNet.Helpers
{
    public sealed class DelegateReturnHelperDescriptor : ReturnHelperDescriptor
    {
        private readonly HandlebarsReturnHelper _helper;

        public DelegateReturnHelperDescriptor(string name, HandlebarsReturnHelper helper) : base(name) => _helper = helper;

        protected override object Invoke(in HelperOptions options, object context, in Arguments arguments) => _helper(context, arguments);
    }
}