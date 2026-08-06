using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using HandlebarsDotNet.Iterators;
using HandlebarsDotNet.MemberAccessors;

namespace HandlebarsDotNet.ObjectDescriptors
{
    /// <summary>
    /// Provides support for <see cref="JsonElement"/> produced by <see cref="System.Text.Json.JsonSerializer"/>
    /// when deserializing into <see cref="object"/>.
    /// </summary>
    public sealed class JsonElementObjectDescriptorProvider : IObjectDescriptorProvider
    {
        private static readonly Type Type = typeof(JsonElement);

        private static readonly JsonElementMemberAccessor MemberAccessor = new JsonElementMemberAccessor();

        private static readonly Func<ObjectDescriptor, IIterator> IteratorFactory = _ => new JsonElementIterator();

        private static readonly Func<ObjectDescriptor, object, IEnumerable> GetProperties = (descriptor, arg) =>
        {
            var element = (JsonElement) arg;
            return element.ValueKind == JsonValueKind.Object
                ? element.EnumerateObject().Select(property => property.Name)
                : Enumerable.Empty<string>();
        };

        private static readonly ObjectDescriptor Descriptor = new ObjectDescriptor(Type, MemberAccessor, GetProperties, IteratorFactory);

        public bool TryGetDescriptor(Type type, [NotNullWhen(true)] out ObjectDescriptor? value)
        {
            if (type != Type)
            {
                value = ObjectDescriptor.Empty;
                return false;
            }

            value = Descriptor;
            return true;
        }
    }
}
