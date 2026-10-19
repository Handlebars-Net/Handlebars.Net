using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Nodes;
using HandlebarsDotNet.Iterators;
using HandlebarsDotNet.MemberAccessors;

namespace HandlebarsDotNet.ObjectDescriptors
{
    /// <summary>
    /// Provides support for <see cref="JsonObject"/> and <see cref="JsonValue"/> in templates.
    /// </summary>
    public sealed class JsonNodeObjectDescriptorProvider : IObjectDescriptorProvider
    {
        private static readonly Type Type = typeof(JsonNode);

        private readonly JsonNodeMemberAccessor _memberAccessor;

        private static readonly Func<ObjectDescriptor, IIterator> IteratorFactory = _ => new JsonNodeIterator();

        private static readonly Func<ObjectDescriptor, object, IEnumerable> GetProperties = (descriptor, arg) =>
            arg is JsonObject obj
                ? obj.Select(property => property.Key)
                : Enumerable.Empty<string>();

        public JsonNodeObjectDescriptorProvider() : this(Array.Empty<IMemberAliasProvider>())
        {
        }

        /// <param name="aliasProviders">Consulted for members a <see cref="JsonNode"/> does not resolve itself.</param>
        public JsonNodeObjectDescriptorProvider(IReadOnlyList<IMemberAliasProvider> aliasProviders)
        {
            _memberAccessor = new JsonNodeMemberAccessor(aliasProviders);
        }

        public bool TryGetDescriptor(Type type, [NotNullWhen(true)] out ObjectDescriptor? value)
        {
            if (!Type.IsAssignableFrom(type) || type == typeof(JsonArray))
            {
                value = ObjectDescriptor.Empty;
                return false;
            }

            value = new ObjectDescriptor(type, _memberAccessor, GetProperties, IteratorFactory);
            return true;
        }
    }
}
