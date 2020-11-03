using System;
using System.Diagnostics;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.EqualityComparers;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet
{
	[DebuggerDisplay("undefined")]
	public class UndefinedBindingResult : IEquatable<UndefinedBindingResult>
    {
	    private static readonly LookupSlim<string, GcDeferredValue<string, UndefinedBindingResult>, StringEqualityComparer> Cache 
		    = new LookupSlim<string, GcDeferredValue<string, UndefinedBindingResult>, StringEqualityComparer>(new StringEqualityComparer(StringComparison.OrdinalIgnoreCase));

	    public static UndefinedBindingResult Create(string value) => Cache.GetOrAdd(value, ValueFactory).Value;
	    
	    private static readonly Func<string, GcDeferredValue<string, UndefinedBindingResult>> ValueFactory = s =>
	    {
		    return new GcDeferredValue<string, UndefinedBindingResult>(s, v => new UndefinedBindingResult(v));
	    };

	    public readonly string Value;
	    
	    private UndefinedBindingResult(string value) => Value = value;

	    public override string ToString() => Value;

	    public bool Equals(UndefinedBindingResult other) => Value == other?.Value;

	    public override bool Equals(object obj) => obj is UndefinedBindingResult other && Equals(other);

	    public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
    }
}

