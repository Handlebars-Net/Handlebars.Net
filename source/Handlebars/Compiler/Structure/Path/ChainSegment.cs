using System;
using System.Diagnostics;
using HandlebarsDotNet.Polyfills;

namespace HandlebarsDotNet.Compiler.Structure.Path
{
    [DebuggerDisplay("{Value}")]
    internal struct ChainSegment : IEquatable<ChainSegment>
    {
        public ChainSegment(string value)
        {
            var segmentValue = string.IsNullOrEmpty(value) ? "this" : value.TrimStart('@').Intern();
            var segmentTrimmedValue = TrimSquareBrackets(segmentValue).Intern();

            IsThis = string.IsNullOrEmpty(value) || string.Equals(value, "this", StringComparison.OrdinalIgnoreCase);
            Value = segmentValue;
            IsVariable = !string.IsNullOrEmpty(value) && value.StartsWith("@");
            TrimmedValue = segmentTrimmedValue;
            LowerInvariant = segmentTrimmedValue.ToLowerInvariant().Intern();
        }

        public readonly string Value;
        public readonly string LowerInvariant;
        public readonly string TrimmedValue;
        public readonly bool IsVariable;
        public readonly bool IsThis;

        public override string ToString()
        {
            return Value;
        }

        public bool Equals(ChainSegment other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is ChainSegment other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value != null ? Value.GetHashCode() : 0;
        }

        public static bool operator ==(ChainSegment a, ChainSegment b)
        {
            return a.Equals(b);
        }
        
        public static bool operator !=(ChainSegment a, ChainSegment b)
        {
            return !a.Equals(b);
        }

        public static string TrimSquareBrackets(string key)
        {
            //Only trim a single layer of brackets.
            if (key.StartsWith("[") && key.EndsWith("]"))
            {
                return key.Substring(1, key.Length - 2);
            }

            return key;
        }
    }
}