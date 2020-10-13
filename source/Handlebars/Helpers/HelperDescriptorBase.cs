using System.IO;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class HelperDescriptorBase : IHelperDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        protected HelperDescriptorBase(string name) => Name = PathResolver.GetPathInfo(name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        protected HelperDescriptorBase(PathInfo name) => Name = name;

        public PathInfo Name { get; }
        public abstract HelperType Type { get; }

        internal abstract object ReturnInvoke(BindingContext bindingContext, object context, object[] arguments);

        internal abstract void WriteInvoke(BindingContext bindingContext, TextWriter output, object context, object[] arguments);
        
        /// <summary>
        /// Returns helper name
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;
    }
}