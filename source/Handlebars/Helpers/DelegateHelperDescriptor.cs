using System.IO;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers
{
    public sealed class DelegateHelperDescriptor : HelperDescriptor
    {
        private readonly HandlebarsHelper _helper;

        public DelegateHelperDescriptor(PathInfo name, HandlebarsHelper helper) : base(name)
        {
            _helper = helper;
        }
        
        public DelegateHelperDescriptor(string name, HandlebarsHelper helper) : base(name)
        {
            _helper = helper;
        }

        public override void Invoke(TextWriter output, object context, params object[] arguments)
        {
            _helper(output, context, arguments);
        }
    }
}