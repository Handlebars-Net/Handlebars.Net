using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    public abstract class BlockHelperDescriptorBase : IHelperDescriptor
    {
        protected BlockHelperDescriptorBase(string name) => Name = name;
        
        public PathInfo Name { get; }
        
        public abstract HelperType Type { get; }
        
        public abstract void Invoke(in EncodedTextWriter output, in BlockHelperOptions options, object context, in Arguments arguments);
    }
}