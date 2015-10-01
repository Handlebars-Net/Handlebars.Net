using System;

namespace HandlebarsDotNet.Compiler.Translation.Expression.Accessors.Members
{
    public class ObjectMemberKey
    {
        public Type InstanceType { get; private set; }
        public string MemberName { get; private set; }

        public ObjectMemberKey(Type instanceType, string memberName)
        {
            InstanceType = instanceType;
            MemberName = memberName;
        }
    }
}
