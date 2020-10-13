using System.IO;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    public abstract class ReturnBlockHelperDescriptor : BlockHelperDescriptorBase
    {
        protected ReturnBlockHelperDescriptor(string name) : base(name)
        {
        }
        
        protected ReturnBlockHelperDescriptor(PathInfo name) : base(name)
        {
        }
        
        public sealed override HelperType Type { get; } = HelperType.ReturnBlock;
        
        public abstract object Invoke(HelperOptions options, object context, params object[] arguments);

        public sealed override void Invoke(TextWriter output, HelperOptions options, object context, params object[] arguments) => 
            output.Write(Invoke(options, context, arguments));
    }
}