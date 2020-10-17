using System.IO;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers
{
    public abstract class HelperDescriptorBase : IHelperDescriptor
    {
        protected HelperDescriptorBase(string name) => Name = PathResolver.GetPathInfo(name);
        
        protected HelperDescriptorBase(PathInfo name) => Name = name;

        public PathInfo Name { get; }
        public abstract HelperType Type { get; }

        internal abstract object ReturnInvoke(BindingContext bindingContext, object context, object[] arguments);

        internal abstract void WriteInvoke(BindingContext bindingContext, TextWriter output, object context, object[] arguments);
        
        public override string ToString() => Name;
    }
}