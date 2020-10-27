using System.IO;
using HandlebarsDotNet.Compiler;
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
        
        public abstract object Invoke(in HelperOptions options, object context, in Arguments arguments);

        public sealed override void Invoke(in EncodedTextWriter output, in HelperOptions options, object context, in Arguments arguments) => 
            output.Write(Invoke(options, context, arguments));
    }
}