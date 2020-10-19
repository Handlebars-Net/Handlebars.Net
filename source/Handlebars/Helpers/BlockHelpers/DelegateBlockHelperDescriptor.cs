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
        
        public override void Invoke(in EncodedTextWriter output, in HelperOptions options, object context, in Arguments arguments) 
            => _helper(output, options, context, arguments);
    }
}