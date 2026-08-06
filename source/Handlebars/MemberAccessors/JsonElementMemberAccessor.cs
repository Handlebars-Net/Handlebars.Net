using System.Text.Json;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.MemberAccessors
{
    public sealed class JsonElementMemberAccessor : IMemberAccessor
    {
        public bool TryGetValue(object instance, ChainSegment memberName, out object? value)
        {
            value = null;

            var element = (JsonElement) instance;
            if (element.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

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

            return false;
        }
    }
}
