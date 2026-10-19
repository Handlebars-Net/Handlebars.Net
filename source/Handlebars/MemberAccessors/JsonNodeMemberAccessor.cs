using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.MemberAccessors
{
    public sealed class JsonNodeMemberAccessor : IMemberAccessor
    {
        private readonly IReadOnlyList<IMemberAliasProvider> _aliasProviders;

        public JsonNodeMemberAccessor() : this(Array.Empty<IMemberAliasProvider>())
        {
        }

        public JsonNodeMemberAccessor(IReadOnlyList<IMemberAliasProvider> aliasProviders)
        {
            _aliasProviders = aliasProviders;
        }

        public bool TryGetValue(object instance, ChainSegment memberName, out object? value)
        {
            if (instance is JsonObject obj && TryGetPropertyValue(obj, memberName, out value))
            {
                return true;
            }

            var type = instance.GetType();
            for (var index = 0; index < _aliasProviders.Count; index++)
            {
                if (_aliasProviders[index].TryGetMemberByAlias(instance, type, memberName, out value))
                {
                    return true;
                }
            }

            value = null;
            return false;
        }

        private static bool TryGetPropertyValue(JsonObject obj, ChainSegment memberName, out object? value)
        {
            if (obj.TryGetPropertyValue(memberName.TrimmedValue, out var property))
            {
                value = property;
                return true;
            }

            if (memberName.LowerInvariant != memberName.TrimmedValue && obj.TryGetPropertyValue(memberName.LowerInvariant, out property))
            {
                value = property;
                return true;
            }

            value = null;
            return false;
        }
    }
}
