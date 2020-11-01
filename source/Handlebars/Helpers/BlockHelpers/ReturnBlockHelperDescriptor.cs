namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    public abstract class ReturnBlockHelperDescriptor : BlockHelperDescriptorBase
    {
        protected ReturnBlockHelperDescriptor(string name) : base(name)
        {
        }

        public sealed override HelperType Type { get; } = HelperType.ReturnBlock;

        protected abstract object Invoke(in BlockHelperOptions options, object context, in Arguments arguments);

        public sealed override void Invoke(in EncodedTextWriter output, in BlockHelperOptions options, object context, in Arguments arguments) => 
            output.Write(Invoke(options, context, arguments));
    }
}