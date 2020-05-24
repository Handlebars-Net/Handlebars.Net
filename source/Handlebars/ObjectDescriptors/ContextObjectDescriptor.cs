using System;
using System.Collections.Generic;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.MemberAccessors;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal class ContextObjectDescriptor : IObjectDescriptorProvider
    {
        private static readonly Type BindingContextType = typeof(BindingContext);
        private static readonly string[] Properties = { "root", "parent" };
        private static readonly Func<ObjectDescriptor, object, IEnumerable<object>> PropertiesDelegate = (descriptor, o) => Properties;

        private static readonly ObjectDescriptor Descriptor =
            new ObjectDescriptor(BindingContextType, new ContextMemberAccessor(), PropertiesDelegate);

        public bool CanHandleType(Type type)
        {
            return type == BindingContextType;
        }

        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            value = Descriptor;
            return true;
        }
    }
}