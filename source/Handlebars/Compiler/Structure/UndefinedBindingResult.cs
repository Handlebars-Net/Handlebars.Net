using System.Diagnostics;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Compiler
{
    [DebuggerDisplay("undefined")]
    internal class UndefinedBindingResult
    {
	    public readonly string Value;
	    private readonly ICompiledHandlebarsConfiguration _configuration;

	    public UndefinedBindingResult(string value, ICompiledHandlebarsConfiguration configuration)
	    {
		    Value = value;
		    _configuration = configuration;
	    }
	    
	    public UndefinedBindingResult(ChainSegment value, ICompiledHandlebarsConfiguration configuration)
	    {
		    Value = value;
		    _configuration = configuration;
	    }

	    public override string ToString()
        {
	        var formatter = _configuration.UnresolvedBindingFormatter;
	        if (formatter == null)
	        {
		        if(string.IsNullOrEmpty(Value)) return string.Empty;
		        formatter = string.Empty;
	        }
	        
	        return string.Format( formatter, Value );
        }
    }
}

