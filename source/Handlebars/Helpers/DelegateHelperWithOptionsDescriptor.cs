namespace HandlebarsDotNet.Helpers
{
    public sealed class DelegateHelperWithOptionsDescriptor : HelperDescriptor
    {
        private readonly HandlebarsHelperWithOptions _helper;

        public DelegateHelperWithOptionsDescriptor(string name, HandlebarsHelperWithOptions helper) : base(name) => _helper = helper;

        protected override void Invoke(in EncodedTextWriter output, in HelperOptions options, object context, in Arguments arguments) => _helper(output, options, context, arguments);
    }
}