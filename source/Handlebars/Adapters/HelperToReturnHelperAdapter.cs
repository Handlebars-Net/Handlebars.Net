namespace HandlebarsDotNet.Adapters
{
    // Will be removed in next iterations
    internal class HelperToReturnHelperAdapter
    {
        private readonly HandlebarsHelper _helper;
        private readonly HandlebarsReturnHelper _delegate;

        public HelperToReturnHelperAdapter(HandlebarsHelper helper)
        {
            _helper = helper;
            _delegate = (context, arguments) =>
            {
                using (var writer = new PolledStringWriter())
                {
                    _helper(writer, context, arguments);
                    return writer.ToString();
                }
            };
        }

        public static implicit operator HandlebarsReturnHelper(HelperToReturnHelperAdapter adapter)
        {
            return adapter._delegate;
        }
    }
}