using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.ObjectDescriptors;

namespace HandlebarsDotNet.Helpers
{
    internal sealed class LookupReturnHelperDescriptor : ReturnHelperDescriptor
    {
        private readonly ICompiledHandlebarsConfiguration _configuration;

        public LookupReturnHelperDescriptor(ICompiledHandlebarsConfiguration configuration) : base(configuration.PathInfoStore.GetOrAdd("lookup"))
        {
            _configuration = configuration;
        }

        public override object Invoke(object context, params object[] arguments)
        {
            if (arguments.Length != 2)
            {
                throw new HandlebarsException("{{lookup}} helper must have exactly two argument");
            }
            
            var segment = ChainSegment.Create(arguments[1]);

            if (ObjectDescriptor.TryCreate(arguments[0], _configuration, out var descriptor))
            {
                return !PathResolver.TryAccessMember(arguments[0], segment, _configuration, out var value) 
                    ? segment.GetUndefinedBindingResult(_configuration)
                    : value;
            }

            return segment.GetUndefinedBindingResult(_configuration);
        }
    }
}