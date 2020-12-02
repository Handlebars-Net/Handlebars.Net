using System;
using System.Diagnostics;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.EqualityComparers;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet
{
	public sealed class UndefinedBindingResultCache
	{
		private static readonly Func<string, DeferredValue<string, UndefinedBindingResult>> ValueFactory = s =>
		{
			return new DeferredValue<string, UndefinedBindingResult>(s, v => new UndefinedBindingResult(v));
		};
		
		private readonly LookupSlim<string, DeferredValue<string, UndefinedBindingResult>, StringEqualityComparer> _cache 
			= new LookupSlim<string, DeferredValue<string, UndefinedBindingResult>, StringEqualityComparer>(new StringEqualityComparer(StringComparison.Ordinal));

		public static UndefinedBindingResultCache Current => AmbientContext.Current?.UndefinedBindingResultCache;

		internal UndefinedBindingResultCache()
		{
			
		}
		
		public UndefinedBindingResult Create(string value) => _cache.GetOrAdd(value, ValueFactory).Value;
	} 
	
	[DebuggerDisplay("undefined")]
	public sealed class UndefinedBindingResult : IEquatable<UndefinedBindingResult>
    {
	    public static UndefinedBindingResult Create(string value) => UndefinedBindingResultCache.Current?.Create(value) ?? new UndefinedBindingResult(value);

	    public readonly string Value;
	    
	    internal UndefinedBindingResult(string value) => Value = value;

	    public override string ToString() => Value;

	    public bool Equals(UndefinedBindingResult other) => Value == other?.Value;

	    public override bool Equals(object obj) => obj is UndefinedBindingResult other && Equals(other);

	    public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
    }
}

