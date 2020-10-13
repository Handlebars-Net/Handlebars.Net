using System.IO;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DelegateBlockHelperDescriptor : BlockHelperDescriptor
    {
        private readonly HandlebarsBlockHelper _helper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="helper"></param>
        public DelegateBlockHelperDescriptor(string name, HandlebarsBlockHelper helper) : base(name)
        {
            _helper = helper;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="helper"></param>
        public DelegateBlockHelperDescriptor(PathInfo name, HandlebarsBlockHelper helper) : base(name)
        {
            _helper = helper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        /// <param name="options"></param>
        /// <param name="context"></param>
        /// <param name="arguments"></param>
        public override void Invoke(TextWriter output, HelperOptions options, object context, params object[] arguments)
        {
            _helper(output, options, context, arguments);
        }
    }
}