using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BlockHelperDescriptor : BlockHelperDescriptorBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        protected BlockHelperDescriptor(string name) : base(name)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        protected BlockHelperDescriptor(PathInfo name) : base(name)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed override HelperType Type { get; } = HelperType.WriteBlock;
    }
}