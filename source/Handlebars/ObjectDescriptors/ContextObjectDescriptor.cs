using System;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.MemberAccessors;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal class ContextObjectDescriptor : IObjectDescriptorProvider
    {
        private static readonly Type BindingContextType = typeof(BindingContext);
        private static readonly string[] Properties = { "root", "parent", "value" };

        private static readonly ObjectDescriptor Descriptor = new ObjectDescriptor(BindingContextType)
        {
            MemberAccessor = new ContextMemberAccessor(),
            GetProperties = o => Properties
        };

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