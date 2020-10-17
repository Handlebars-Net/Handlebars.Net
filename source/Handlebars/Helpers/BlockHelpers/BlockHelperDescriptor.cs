using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    public abstract class BlockHelperDescriptor : BlockHelperDescriptorBase
    {
        protected BlockHelperDescriptor(string name) : base(name)
        {
        }
        
        protected BlockHelperDescriptor(PathInfo name) : base(name)
        {
        }
        
        public sealed override HelperType Type { get; } = HelperType.WriteBlock;
    }
}