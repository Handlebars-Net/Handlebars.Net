using System;
using System.Diagnostics;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Polyfills;

namespace HandlebarsDotNet.Compiler.Structure.Path
{
    [DebuggerDisplay("{Value}")]
    internal struct ChainSegment
    {
        private static readonly RefLookup<string, ChainSegment> ChainSegments = new RefLookup<string, ChainSegment>();
        
        public static ref ChainSegment Create(string value)
        {
            if (ChainSegments.ContainsKey(value))
            {
                return ref ChainSegments.GetValueOrDefault(value);
            }
            
            return ref ChainSegments.GetOrAdd(value, (string key, ref ChainSegment segment) =>
            {
                segment.IsThis = string.IsNullOrEmpty(value) || string.Equals(value, "this", StringComparison.OrdinalIgnoreCase);
                segment.Value = string.IsNullOrEmpty(value) ? "this" : value.TrimStart('@').Intern();
                segment.IsVariable = !string.IsNullOrEmpty(value) && value.StartsWith("@");
                segment.TrimmedValue = TrimSquareBrackets(segment.Value).Intern();
                segment.LowerInvariant = segment.TrimmedValue.ToLowerInvariant().Intern();

                return ref segment;
            });
        }

        public string Value { get; private set; }
        public string LowerInvariant { get; private set; }
        public string TrimmedValue { get; private set; }
        public bool IsVariable { get; private set; }
        public bool IsThis { get; private set; }

        private static string TrimSquareBrackets(string key)
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