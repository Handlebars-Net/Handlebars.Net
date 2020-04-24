using System;
using System.Diagnostics;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Polyfills;

namespace HandlebarsDotNet.Compiler.Structure.Path
{
    [DebuggerDisplay("{Value}")]
    internal struct ChainSegment
    {
        public static ChainSegment Create(string value)
        {
            var segmentValue = string.IsNullOrEmpty(value) ? "this" : value.TrimStart('@').Intern();
            var segmentTrimmedValue = TrimSquareBrackets(segmentValue).Intern();

            return new ChainSegment
            {
                IsThis = string.IsNullOrEmpty(value) || string.Equals(value, "this", StringComparison.OrdinalIgnoreCase),
                Value = segmentValue,
                IsVariable = !string.IsNullOrEmpty(value) && value.StartsWith("@"),
                TrimmedValue = segmentTrimmedValue,
                LowerInvariant = segmentTrimmedValue.ToLowerInvariant().Intern()
            };
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