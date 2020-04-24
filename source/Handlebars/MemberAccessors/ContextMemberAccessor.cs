using System;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.MemberAccessors
{
    internal class ContextMemberAccessor : IMemberAccessor
    {
        public bool TryGetValue(object instance, Type instanceType, string memberName, out object value)
        {
            var bindingContext = (BindingContext) instance;
            var segment = ChainSegment.Create(memberName);
            return bindingContext.TryGetContextVariable(ref segment, out value);
        }
    }
}