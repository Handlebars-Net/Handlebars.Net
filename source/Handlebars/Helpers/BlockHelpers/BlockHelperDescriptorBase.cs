using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    public abstract class BlockHelperDescriptorBase : IHelperDescriptor
    {
        protected BlockHelperDescriptorBase(string name) => Name = PathInfoStore.Shared.GetOrAdd(name);

        protected BlockHelperDescriptorBase(PathInfo name) => Name = name;

        public PathInfo Name { get; }
        
        public abstract HelperType Type { get; }
        
        public abstract void Invoke(in EncodedTextWriter output, in HelperOptions options, object context, in Arguments arguments);
    }
}