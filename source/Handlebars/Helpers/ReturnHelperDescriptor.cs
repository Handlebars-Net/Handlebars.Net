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
        
        public abstract object Invoke(object context, params object[] arguments);

        internal override object ReturnInvoke(BindingContext bindingContext, object context, object[] arguments) => 
            Invoke(context, arguments);

        internal sealed override void WriteInvoke(BindingContext bindingContext, TextWriter output, object context, object[] arguments) => 
            output.Write(ReturnInvoke(bindingContext, context, arguments));
    }
}