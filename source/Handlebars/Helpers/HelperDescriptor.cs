using System.IO;
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

        public abstract void Invoke(TextWriter output, object context, params object[] arguments);

        internal sealed override object ReturnInvoke(BindingContext bindingContext, object context, object[] arguments)
        {
            using var writer = ReusableStringWriter.Get(bindingContext.Configuration.FormatProvider);
            WriteInvoke(bindingContext, writer, context, arguments);
            return writer.ToString();
        }
        
        internal sealed override void WriteInvoke(BindingContext bindingContext, TextWriter output, object context, object[] arguments) => Invoke(output, context, arguments);
    }
}