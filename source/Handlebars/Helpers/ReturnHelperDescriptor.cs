using HandlebarsDotNet.Compiler;

namespace HandlebarsDotNet.Helpers
{
    public abstract class ReturnHelperDescriptor : HelperDescriptorBase
    {
        protected ReturnHelperDescriptor(string name) : base(name)
        {
        }
        
        public sealed override HelperType Type { get; } = HelperType.Return;

        protected abstract object Invoke(in HelperOptions options, object context, in Arguments arguments);

        internal sealed override object ReturnInvoke(BindingContext bindingContext, object context, in Arguments arguments)
        {
            var options = new HelperOptions(bindingContext);
            return Invoke(options, context, arguments);
        }

        internal sealed override void WriteInvoke(BindingContext bindingContext, in EncodedTextWriter output, object context, in Arguments arguments) => 
            output.Write(ReturnInvoke(bindingContext, context, arguments));
    }
}