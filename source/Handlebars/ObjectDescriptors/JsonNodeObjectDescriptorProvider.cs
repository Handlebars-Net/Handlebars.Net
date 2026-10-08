using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using HandlebarsDotNet.Iterators;
using HandlebarsDotNet.MemberAccessors;

namespace HandlebarsDotNet.ObjectDescriptors
{
    /// <summary>
    /// Provides support for <see cref="JsonObject"/>, <see cref="JsonArray"/>, and <see cref="JsonValue"/> in templates.
    /// </summary>
    public sealed class JsonNodeObjectDescriptorProvider : IObjectDescriptorProvider
    {
        private readonly EnumerableObjectDescriptor _enumerableObjectDescriptor;

        private static readonly Type Type = typeof(JsonNode);

        private static readonly JsonNodeMemberAccessor MemberAccessor = new JsonNodeMemberAccessor();

        private static readonly Func<ObjectDescriptor, IIterator> IteratorFactory = _ => new JsonNodeIterator();

        private static readonly Func<ObjectDescriptor, object, IEnumerable> GetProperties = (descriptor, arg) =>
        {
            switch (arg)
            {
                case JsonObject obj:
                    return obj.Select(property => property.Key);
                // A JsonValue wrapping a CLR collection or object: list the underlying value's properties,
                // matching JsonNodeMemberAccessor and JsonNodeIterator.
                case JsonValue wrapped when wrapped.GetValueKind() is JsonValueKind.Array or JsonValueKind.Object:
                    var underlying = wrapped.GetValue<object>();
                    return ObjectDescriptor.TryCreate(underlying, out var underlyingDescriptor)
                        ? underlyingDescriptor.GetProperties(underlyingDescriptor, underlying)
                        : Enumerable.Empty<string>();
                default:
                    return Enumerable.Empty<string>();
            }
        };

        public JsonNodeObjectDescriptorProvider(EnumerableObjectDescriptor enumerableObjectDescriptor)
        {
            _enumerableObjectDescriptor = enumerableObjectDescriptor;
        }

        public bool TryGetDescriptor(Type type, [NotNullWhen(true)] out ObjectDescriptor? value)
        {
            if (!Type.IsAssignableFrom(type))
            {
                value = ObjectDescriptor.Empty;
                return false;
            }

            if (type == typeof(JsonArray) && _enumerableObjectDescriptor.TryGetDescriptor(type, out var arrayDescriptor))
            {
                value = new ObjectDescriptor(type, arrayDescriptor.MemberAccessor, arrayDescriptor.GetProperties, IteratorFactory, arrayDescriptor.Dependencies);
                return true;
            }

            value = new ObjectDescriptor(type, MemberAccessor, GetProperties, IteratorFactory);
            return true;
        }
    }
}
