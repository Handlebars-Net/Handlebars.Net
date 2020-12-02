using System;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.EqualityComparers;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet.PathStructure
{
    public sealed class ChainSegmentStore
    {
        private static readonly Func<string, WellKnownVariable, DeferredValue<CreationProperties, ChainSegment>> ValueFactory = (s, v) =>
        {
            return new DeferredValue<CreationProperties, ChainSegment>(new CreationProperties(s, v), properties => new ChainSegment(properties.String, properties.KnownVariable));
        };

        public static ChainSegmentStore Current => AmbientContext.Current?.ChainSegmentStore;
        
        private readonly LookupSlim<string, DeferredValue<CreationProperties, ChainSegment>, StringEqualityComparer> _lookup = new LookupSlim<string, DeferredValue<CreationProperties, ChainSegment>, StringEqualityComparer>(new StringEqualityComparer(StringComparison.Ordinal));

        internal ChainSegmentStore()
        {
            
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ChainSegment Create(string value) => _lookup.GetOrAdd(value, ValueFactory, WellKnownVariable.None).Value;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ChainSegment Create(object value)
        {
            if (value is ChainSegment segment) return segment;
            return _lookup.GetOrAdd(value as string ?? value.ToString(), ValueFactory, WellKnownVariable.None).Value;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ChainSegment Create(string value, WellKnownVariable variable) => _lookup.GetOrAdd(value, ValueFactory, variable).Value;

        internal readonly struct CreationProperties
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