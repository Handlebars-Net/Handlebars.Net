using System.IO;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers
{
    public abstract class ReturnHelperDescriptor : HelperDescriptorBase
    {
        protected ReturnHelperDescriptor(PathInfo name) : base(name)
        {
        }
        
        protected ReturnHelperDescriptor(string name) : base(name)
        {
        }
        
        public sealed override HelperType Type { get; } = HelperType.Return;
        
        public abstract object Invoke(object context, in Arguments arguments);

        internal override object ReturnInvoke(BindingContext bindingContext, object context, in Arguments arguments) => 
            Invoke(context, arguments);

        internal sealed override void WriteInvoke(BindingContext bindingContext, in EncodedTextWriter output, object context, in Arguments arguments) => 
            output.Write(ReturnInvoke(bindingContext, context, arguments));
    }
}