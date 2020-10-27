using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers
{
    public abstract class HelperDescriptorBase : IHelperDescriptor
    {
        protected HelperDescriptorBase(string name) => Name = PathInfoStore.Shared.GetOrAdd(name);
        
        protected HelperDescriptorBase(PathInfo name) => Name = name;

        public PathInfo Name { get; }
        public abstract HelperType Type { get; }
        
        internal abstract object ReturnInvoke(BindingContext bindingContext, object context, in Arguments arguments);

        internal abstract void WriteInvoke(BindingContext bindingContext, in EncodedTextWriter output, object context, in Arguments arguments);

        public override string ToString() => Name;
    }
}