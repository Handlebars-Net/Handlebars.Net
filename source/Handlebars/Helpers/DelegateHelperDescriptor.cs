namespace HandlebarsDotNet.Helpers
{
    public sealed class DelegateHelperDescriptor : HelperDescriptor
    {
        private readonly HandlebarsHelper _helper;

        public DelegateHelperDescriptor(string name, HandlebarsHelper helper) : base(name) => _helper = helper;

        protected override void Invoke(in EncodedTextWriter output, in HelperOptions options, object context, in Arguments arguments) => _helper(output, context, arguments);
    }
}