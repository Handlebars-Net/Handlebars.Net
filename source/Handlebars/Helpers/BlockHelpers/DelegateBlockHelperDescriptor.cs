using System.IO;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    public sealed class DelegateBlockHelperDescriptor : BlockHelperDescriptor
    {
        private readonly HandlebarsBlockHelper _helper;
        
        public DelegateBlockHelperDescriptor(string name, HandlebarsBlockHelper helper) : base(name) 
            => _helper = helper;
        
        public DelegateBlockHelperDescriptor(PathInfo name, HandlebarsBlockHelper helper) : base(name)
        {
            _helper = helper;
        }
        
        public override void Invoke(TextWriter output, HelperOptions options, object context, params object[] arguments) 
            => _helper(output, options, context, arguments);
    }
}