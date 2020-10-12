using System;
using HandlebarsDotNet.Polyfills;

namespace HandlebarsDotNet.Compiler.Structure.Path
{
    /// <summary>
    /// Represents parts of single <see cref="PathSegment"/> separated with dots.
    /// </summary>
    public struct ChainSegment : IEquatable<ChainSegment>
    {
        private readonly string _value;
        
        internal readonly string LowerInvariant;
        
        public ChainSegment(string value)
        {
            var segmentValue = string.IsNullOrEmpty(value) ? "this" : value.TrimStart('@').Intern();
            var segmentTrimmedValue = TrimSquareBrackets(segmentValue).Intern();

            IsThis = string.IsNullOrEmpty(value) || string.Equals(value, "this", StringComparison.OrdinalIgnoreCase);
            _value = segmentValue;
            IsVariable = !string.IsNullOrEmpty(value) && value.StartsWith("@");
            TrimmedValue = segmentTrimmedValue;
            LowerInvariant = segmentTrimmedValue.ToLowerInvariant().Intern();
        }

        /// <summary>
        /// Value with trimmed '[' and ']'
        /// </summary>
        public readonly string TrimmedValue;
        
        /// <summary>
        /// Indicates whether <see cref="ChainSegment"/> is part of <c>@</c> variable
        /// </summary>
        public readonly bool IsVariable;
        
        /// <summary>
        /// Indicates whether <see cref="ChainSegment"/> is <c>this</c> or <c>.</c>
        /// </summary>
        public readonly bool IsThis;

        /// <summary>
        /// Returns string representation of current <see cref="ChainSegment"/>
        /// </summary>
        public override string ToString() => _value;

        /// <inheritdoc />
        public bool Equals(ChainSegment other) => _value == other._value;

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is ChainSegment other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => _value != null ? _value.GetHashCode() : 0;

        /// <inheritdoc cref="Equals(HandlebarsDotNet.Compiler.Structure.Path.ChainSegment)"/>
        public static bool operator ==(ChainSegment a, ChainSegment b) => a.Equals(b);

        /// <inheritdoc cref="Equals(HandlebarsDotNet.Compiler.Structure.Path.ChainSegment)"/>
        public static bool operator !=(ChainSegment a, ChainSegment b) => !a.Equals(b);

        /// <inheritdoc cref="ToString"/>
        public static implicit operator string(ChainSegment segment) => segment._value;

        internal static string TrimSquareBrackets(string key)
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