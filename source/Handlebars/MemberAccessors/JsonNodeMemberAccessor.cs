using System.Text.Json;
using System.Text.Json.Nodes;
using HandlebarsDotNet.ObjectDescriptors;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.MemberAccessors
{
    public sealed class JsonNodeMemberAccessor : IMemberAccessor
    {
        public bool TryGetValue(object instance, ChainSegment memberName, out object? value)
        {
            value = null;

            switch (instance)
            {
                case JsonObject obj:
                    return TryGetPropertyValue(obj, memberName, out value);
                // A JsonValue wrapping a CLR collection or object (e.g. JsonValue.Create(new List<string>())):
                // resolve members on the underlying value with its own accessor, matching JsonNodeIterator.
                // GetValueKind() serializes the wrapped value; System.Text.Json 9+ caches the result per node, 8.x does not.
                case JsonValue wrapped when wrapped.GetValueKind() is JsonValueKind.Array or JsonValueKind.Object:
                    var underlying = wrapped.GetValue<object>();
                    return ObjectDescriptor.TryCreate(underlying, out var descriptor)
                        && descriptor.MemberAccessor.TryGetValue(underlying, memberName, out value);
                default:
                    return false;
            }
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
