using System.IO;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers
{
    public abstract class ReturnHelperDescriptor : HelperDescriptorBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        protected ReturnHelperDescriptor(PathInfo name) : base(name)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        protected ReturnHelperDescriptor(string name) : base(name)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed override HelperType Type { get; } = HelperType.Return;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public abstract object Invoke(object context, params object[] arguments);

        internal override object ReturnInvoke(BindingContext bindingContext, object context, object[] arguments) => 
            Invoke(context, arguments);

        internal sealed override void WriteInvoke(BindingContext bindingContext, TextWriter output, object context, object[] arguments) => 
            output.Write(ReturnInvoke(bindingContext, context, arguments));
    }
}