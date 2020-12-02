using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.StringUtils;

namespace HandlebarsDotNet.PathStructure
{
    /// <summary>
    /// Represents parts of single <see cref="PathSegment"/> separated with dots.
    /// </summary>
    public sealed partial class ChainSegment : IEquatable<ChainSegment>
    {
        public static ChainSegmentEqualityComparer EqualityComparer { get; } = new ChainSegmentEqualityComparer();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ChainSegment Create(string value) => ChainSegmentStore.Current?.Create(value) ?? new ChainSegment(value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ChainSegment Create(object value)
        {
            if (value is ChainSegment segment) return segment;
            var sValue = value as string ?? value.ToString();
            return ChainSegmentStore.Current?.Create(sValue) ?? new ChainSegment(sValue);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ChainSegment Create(string value, WellKnownVariable variable)
        {
            return ChainSegmentStore.Current?.Create(value, variable) ?? new ChainSegment(value, variable);
        }

        public static ChainSegment Index { get; } = Create(nameof(Index), WellKnownVariable.Index);
        public static ChainSegment First { get; } = Create(nameof(First), WellKnownVariable.First);
        public static ChainSegment Last { get; } = Create(nameof(Last), WellKnownVariable.Last);
        public static ChainSegment Value { get; } = Create(nameof(Value), WellKnownVariable.Value);
        public static ChainSegment Key { get; } = Create(nameof(Key), WellKnownVariable.Key);
        public static ChainSegment Root { get; } = Create(nameof(Root), WellKnownVariable.Root);
        public static ChainSegment Parent { get; } = Create(nameof(Parent), WellKnownVariable.Parent);
        public static ChainSegment This { get; } = Create(nameof(This), WellKnownVariable.This);
        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly int _hashCode;
        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _value;

        /// <summary>
        ///  
        /// </summary>
        internal ChainSegment(string value, WellKnownVariable wellKnownVariable = WellKnownVariable.None)
        {
            WellKnownVariable = wellKnownVariable;

            var isNullOrEmpty = string.IsNullOrEmpty(value);
            var segmentValue = isNullOrEmpty ? new Substring("this") : new Substring(value);
            var segmentTrimmedValue = TrimSquareBrackets(segmentValue);

            _value = value;
            IsThis = isNullOrEmpty || string.Equals(value, "this", StringComparison.OrdinalIgnoreCase);
            TrimmedValue = segmentTrimmedValue.ToString();
            LowerInvariant = TrimmedValue.ToLowerInvariant();
            
            IsValue = LowerInvariant == "value";

            _hashCode = GetHashCodeImpl();

            if (IsThis) WellKnownVariable = WellKnownVariable.This;
            if (IsValue) WellKnownVariable = WellKnownVariable.Value;
        }

        /// <summary>
        /// Value with trimmed '[' and ']'
        /// </summary>
        public readonly string TrimmedValue;
        
        /// <summary>
        /// Indicates whether <see cref="ChainSegment"/> is <c>this</c> or <c>.</c>
        /// </summary>
        public readonly bool IsThis;

        internal readonly string LowerInvariant;
        internal readonly bool IsValue;
        
        internal readonly WellKnownVariable WellKnownVariable;

        /// <summary>
        /// Returns string representation of current <see cref="ChainSegment"/>
        /// </summary>
        public override string ToString() => _value;

        /// <inheritdoc />
        public bool Equals(ChainSegment other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualsImpl(other);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is ChainSegment segment)) return false;
            return EqualsImpl(segment);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool EqualsImpl(ChainSegment other)
        {
            return _hashCode == other._hashCode 
                   && IsThis == other.IsThis 
                   && LowerInvariant == other.LowerInvariant;
        }

        /// <inheritdoc />
        public override int GetHashCode() => _hashCode;

        private int GetHashCodeImpl()
        {
            unchecked
            {
                var hashCode = IsThis.GetHashCode();
                hashCode = (hashCode * 397) ^ (LowerInvariant.GetHashCode());
                return hashCode;
            }
        }

        /// <inheritdoc cref="Equals(HandlebarsDotNet.PathStructure.ChainSegment)"/>
        public static bool operator ==(ChainSegment a, ChainSegment b) => Equals(a, b);

        /// <inheritdoc cref="Equals(HandlebarsDotNet.PathStructure.ChainSegment)"/>
        public static bool operator !=(ChainSegment a, ChainSegment b) => !Equals(a, b);

        /// <inheritdoc cref="ToString"/>
        public static implicit operator string(ChainSegment segment) => segment._value;
        
        public static implicit operator ChainSegment(string segment) => Create(segment);

        private static Substring TrimSquareBrackets(Substring key)
        {
            //Only trim a single layer of brackets.
            if (key.StartsWith('[') && key.EndsWith(']'))
            {
                return new Substring(key, 1, key.Length - 2);
            }

            return key;
        }
    }
}