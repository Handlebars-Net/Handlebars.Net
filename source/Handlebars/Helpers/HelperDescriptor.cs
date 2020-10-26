using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers
{
    public abstract class HelperDescriptor : HelperDescriptorBase
    {
        protected HelperDescriptor(string name) : base(name)
        {
        }
        
        protected HelperDescriptor(PathInfo name) : base(name)
        {
        }

        public sealed override HelperType Type { get; } = HelperType.Write;

        public abstract void Invoke(in EncodedTextWriter output, object context, in Arguments arguments);

        internal sealed override object ReturnInvoke(BindingContext bindingContext, object context, in Arguments arguments)
        {
            var configuration = bindingContext.Configuration;
            using var writer = ReusableStringWriter.Get(configuration.FormatProvider);
            using var encodedTextWriter = new EncodedTextWriter(writer, configuration.TextEncoder, configuration.UnresolvedBindingFormat, configuration.NoEscape);
            WriteInvoke(bindingContext, encodedTextWriter, context, arguments);
            return writer.ToString();
        }
        
        internal sealed override void WriteInvoke(BindingContext bindingContext, in EncodedTextWriter output, object context, in Arguments arguments) => Invoke(output, context, arguments);
    }
}