using System;
using System.Diagnostics;
using HandlebarsDotNet.Collections;

namespace HandlebarsDotNet
{
	[DebuggerDisplay("undefined")]
	public class UndefinedBindingResult : IEquatable<UndefinedBindingResult>
    {
	    // TODO: migrate to WeakReference?
	    private static readonly LookupSlim<string, UndefinedBindingResult> Cache = new LookupSlim<string, UndefinedBindingResult>();

	    public static UndefinedBindingResult Create(string value) => Cache.GetOrAdd(value, s => new UndefinedBindingResult(s));
	    

	    public readonly string Value;

	    private UndefinedBindingResult(string value) => Value = value;

	    public override string ToString() => string.Empty;

	    public bool Equals(UndefinedBindingResult other) => Value == other.Value;

	    public override bool Equals(object obj) => obj is UndefinedBindingResult other && Equals(other);

	    public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
    }
}

