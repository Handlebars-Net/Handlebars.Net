using System;
using System.Collections.Generic;
using System.Text.Json;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.MemberAccessors
{
    public sealed class JsonElementMemberAccessor : IMemberAccessor
    {
        private static readonly Type Type = typeof(JsonElement);

        private readonly IReadOnlyList<IMemberAliasProvider> _aliasProviders;

        public JsonElementMemberAccessor() : this(Array.Empty<IMemberAliasProvider>())
        {
        }

        public JsonElementMemberAccessor(IReadOnlyList<IMemberAliasProvider> aliasProviders)
        {
            _aliasProviders = aliasProviders;
        }

        public bool TryGetValue(object instance, ChainSegment memberName, out object? value)
        {
            var element = (JsonElement) instance;
            var found = element.ValueKind switch
            {
                JsonValueKind.Object => TryGetPropertyValue(element, memberName, out value),
                JsonValueKind.Array => TryGetArrayMember(element, memberName, out value),
                _ => NotFound(out value)
            };

            return found || TryGetValueByAlias(instance, memberName, out value);
        }

        private static bool TryGetArrayMember(JsonElement element, ChainSegment memberName, out object? value)
        {
            // Same index parsing as EnumerableMemberAccessor: non-negative integers only.
            if (int.TryParse(memberName.LowerInvariant, out var index) && index >= 0 && index < element.GetArrayLength())
            {
                value = element[index];
                return true;
            }

            // Matches the Count property that List<T> and JsonArray expose through reflection;
            // Length is added by the collection alias provider, as for other collections.
            if (memberName.LowerInvariant == "count")
            {
                value = element.GetArrayLength();
                return true;
            }

            return NotFound(out value);
        }

        private bool TryGetValueByAlias(object instance, ChainSegment memberName, out object? value)
        {
            for (var index = 0; index < _aliasProviders.Count; index++)
            {
                if (_aliasProviders[index].TryGetMemberByAlias(instance, Type, memberName, out value))
                    return true;
            }

            return NotFound(out value);
        }

        private static bool NotFound(out object? value)
        {
            value = null;
            return false;
        }

        private static bool TryGetPropertyValue(JsonElement element, ChainSegment memberName, out object? value)
        {
            if (element.TryGetProperty(memberName.TrimmedValue, out var property))
            {
                value = property;
                return true;
            }

            if (memberName.LowerInvariant != memberName.TrimmedValue && element.TryGetProperty(memberName.LowerInvariant, out property))
            {
                value = property;
                return true;
            }

            return NotFound(out value);
        }
    }
}
