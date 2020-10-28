namespace HandlebarsDotNet.Helpers
{
    public sealed class DelegateReturnHelperWithOptionsDescriptor : ReturnHelperDescriptor
    {
        private readonly HandlebarsReturnWithOptionsHelper _helper;

        public DelegateReturnHelperWithOptionsDescriptor(string name, HandlebarsReturnWithOptionsHelper helper) : base(name) => _helper = helper;

        protected override object Invoke(in HelperOptions options, object context, in Arguments arguments) => _helper(options, context, arguments);
    }
}