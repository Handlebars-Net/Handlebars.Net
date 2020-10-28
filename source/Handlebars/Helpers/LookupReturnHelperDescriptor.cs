using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers
{
    internal sealed class LookupReturnHelperDescriptor : ReturnHelperDescriptor
    {
        private readonly ICompiledHandlebarsConfiguration _configuration;

        public LookupReturnHelperDescriptor(ICompiledHandlebarsConfiguration configuration) : base("lookup")
        {
            _configuration = configuration;
        }

        protected override object Invoke(in HelperOptions options, object context, in Arguments arguments)
        {
            if (arguments.Length != 2)
            {
                throw new HandlebarsException("{{lookup}} helper must have exactly two argument");
            }
            
            var segment = ChainSegment.Create(arguments[1]);

            return !PathResolver.TryAccessMember(arguments[0], segment, _configuration, out var value) 
                ? UndefinedBindingResult.Create(segment)
                : value;
        }
    }
}