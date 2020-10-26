using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.Collections;

namespace HandlebarsDotNet.Compiler.Structure.Path
{
    internal enum WellKnownVariable
    {
        None = -1,
        Index = 0,
        Key = 1,
        Value = 2,
        First = 3,
        Last = 4,
        Root = 5,
        Parent = 6,
        This = 7,
    }
    
    /// <summary>
    /// Represents parts of single <see cref="PathSegment"/> separated with dots.
    /// </summary>
    public sealed class ChainSegment : IEquatable<ChainSegment>
    {
        private static readonly char[] TrimStart = {'@'};
        
        // TODO: migrate to WeakReference?
        private static readonly LookupSlim<string, SafeDeferredValue<CreationProperties, ChainSegment>> Lookup = new LookupSlim<string, SafeDeferredValue<CreationProperties, ChainSegment>>();
        
        private static readonly Func<string, WellKnownVariable, SafeDeferredValue<CreationProperties, ChainSegment>> ValueFactory = (s, v) =>
        {
            return new SafeDeferredValue<CreationProperties, ChainSegment>(new CreationProperties(s, v), properties => new ChainSegment(properties.String, properties.KnownVariable));
        };
        
        public static ChainSegmentEqualityComparer EqualityComparer { get; } = new ChainSegmentEqualityComparer();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ChainSegment Create(string value) => Lookup.GetOrAdd(value, ValueFactory, WellKnownVariable.None).Value;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ChainSegment Create(object value)
        {
            if (value is ChainSegment segment) return segment;
            return Lookup.GetOrAdd(value as string ?? value.ToString(), ValueFactory, WellKnownVariable.None).Value;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ChainSegment Create(string value, WellKnownVariable variable, bool createVariable = false)
        {
            if (createVariable)
            {
                Lookup.GetOrAdd($"@{value}", ValueFactory, variable);
            }
            
            return Lookup.GetOrAdd(value, ValueFactory, variable).Value;
        }

        public static ChainSegment Index { get; } = Create(nameof(Index), WellKnownVariable.Index, true);
        public static ChainSegment First { get; } = Create(nameof(First), WellKnownVariable.First, true);
        public static ChainSegment Last { get; } = Create(nameof(Last), WellKnownVariable.Last, true);
        public static ChainSegment Value { get; } = Create(nameof(Value), WellKnownVariable.Value, true);
        public static ChainSegment Key { get; } = Create(nameof(Key), WellKnownVariable.Key, true);
        public static ChainSegment Root { get; } = Create(nameof(Root), WellKnownVariable.Root, true);
        public static ChainSegment Parent { get; } = Create(nameof(Parent), WellKnownVariable.Parent, true);
        public static ChainSegment This { get; } = Create(nameof(This), WellKnownVariable.This);
        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly int _hashCode;
        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _value;

        /// <summary>
        ///  
        /// </summary>
        private ChainSegment(string value, WellKnownVariable wellKnownVariable = WellKnownVariable.None)
        {
            WellKnownVariable = wellKnownVariable;
            
            var segmentValue = string.IsNullOrEmpty(value) ? "this" : value.TrimStart(TrimStart);
            var segmentTrimmedValue = TrimSquareBrackets(segmentValue);

            _value = segmentValue;
            IsThis = string.IsNullOrEmpty(value) || string.Equals(value, "this", StringComparison.OrdinalIgnoreCase);
            IsVariable = !string.IsNullOrEmpty(value) && value.StartsWith("@");
            TrimmedValue = segmentTrimmedValue;
            LowerInvariant = segmentTrimmedValue.ToLowerInvariant();
            
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
        /// Indicates whether <see cref="ChainSegment"/> is part of <c>@</c> variable
        /// </summary>
        public readonly bool IsVariable;
        
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
            if (obj.GetType() != GetType()) return false;
            return EqualsImpl((ChainSegment) obj);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool EqualsImpl(ChainSegment other)
        {
            return IsThis == other.IsThis
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

        /// <inheritdoc cref="Equals(HandlebarsDotNet.Compiler.Structure.Path.ChainSegment)"/>
        public static bool operator ==(ChainSegment a, ChainSegment b) => a.Equals(b);

        /// <inheritdoc cref="Equals(HandlebarsDotNet.Compiler.Structure.Path.ChainSegment)"/>
        public static bool operator !=(ChainSegment a, ChainSegment b) => !a.Equals(b);

        /// <inheritdoc cref="ToString"/>
        public static implicit operator string(ChainSegment segment) => segment._value;
        
        /// <summary>
        /// 
        /// </summary>
        
        public static implicit operator ChainSegment(string segment) => Create(segment);

        private static string TrimSquareBrackets(string key)
        {
            //Only trim a single layer of brackets.
            if (key.StartsWith("[") && key.EndsWith("]"))
            {
                return key.Substring(1, key.Length - 2);
            }

            return key;
        }
        
        public struct ChainSegmentEqualityComparer : IEqualityComparer<ChainSegment>
        {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(ChainSegment x, ChainSegment y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                
                return x._hashCode == y._hashCode && x.IsThis == y.IsThis && x.LowerInvariant == y.LowerInvariant;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int GetHashCode(ChainSegment obj) => obj._hashCode;
        }
        
        private readonly struct CreationProperties
        {
            public readonly string String;
            public readonly WellKnownVariable KnownVariable;

            public CreationProperties(string @string, WellKnownVariable knownVariable = WellKnownVariable.None)
            {
                String = @string;
                KnownVariable = knownVariable;
            }
        }
    }
}