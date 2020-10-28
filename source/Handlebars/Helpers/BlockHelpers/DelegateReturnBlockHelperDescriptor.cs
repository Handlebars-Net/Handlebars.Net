namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    public sealed class DelegateReturnBlockHelperDescriptor : ReturnBlockHelperDescriptor
    {
        private readonly HandlebarsReturnBlockHelper _helper;

        public DelegateReturnBlockHelperDescriptor(string name, HandlebarsReturnBlockHelper helper) : base(name) => _helper = helper;

        protected override object Invoke(in BlockHelperOptions options, object context, in Arguments arguments) => _helper(options, context, arguments);
    }
}