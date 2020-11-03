using HandlebarsDotNet.Compiler;

namespace HandlebarsDotNet.Helpers
{
    public abstract class HelperDescriptor : HelperDescriptorBase
    {
        protected HelperDescriptor(string name) : base(name)
        {
        }

        public sealed override HelperType Type { get; } = HelperType.Write;

        protected abstract void Invoke(in EncodedTextWriter output, in HelperOptions options, object context, in Arguments arguments);

        internal sealed override object ReturnInvoke(BindingContext bindingContext, object context, in Arguments arguments)
        {
            var configuration = bindingContext.Configuration;
            using var writer = ReusableStringWriter.Get(configuration.FormatProvider);
            using var encodedTextWriter = new EncodedTextWriter(writer, configuration.TextEncoder, configuration.UnresolvedBindingFormatter, configuration.NoEscape);
            WriteInvoke(bindingContext, encodedTextWriter, context, arguments);
            return writer.ToString();
        }
        
        internal sealed override void WriteInvoke(BindingContext bindingContext, in EncodedTextWriter output, object context, in Arguments arguments)
        {
            var options = new HelperOptions(bindingContext);
            Invoke(output, options, context, arguments);
        }
    }
}