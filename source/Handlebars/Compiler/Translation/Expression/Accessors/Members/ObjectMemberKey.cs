using System;
using System.Collections.Generic;

namespace HandlebarsDotNet.Compiler.Translation.Expression.Accessors.Members
{
    public class ObjectMemberKey : IEqualityComparer<ObjectMemberKey>
    {
        public Type InstanceType { get; private set; }
        public string MemberName { get; private set; }

        public bool Equals(ObjectMemberKey x, ObjectMemberKey y)
        {
            var typeEquality = x.InstanceType == y.InstanceType;
            var memberName = x.MemberName == y.MemberName;

            return typeEquality && memberName;
        }

        public int GetHashCode(ObjectMemberKey obj)
        {
            //TODO: Review this implementation
            return InstanceType.GetHashCode()
                   + (int) Math.Pow(MemberName.GetHashCode(), 2);
        }
    }
}
