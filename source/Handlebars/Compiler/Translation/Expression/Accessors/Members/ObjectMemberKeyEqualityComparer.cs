using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandlebarsDotNet.Compiler.Translation.Expression.Accessors.Members
{
    internal class ObjectMemberKeyEqualityComparer : IEqualityComparer<ObjectMemberKey>
    {
        public bool Equals(ObjectMemberKey x, ObjectMemberKey y)
        {
            var typeEquality = x.InstanceType == y.InstanceType;
            var memberName = x.MemberName == y.MemberName;

            return typeEquality && memberName;
        }

        public int GetHashCode(ObjectMemberKey obj)
        {
            unchecked
            {
                int hash = 17;

                hash = hash*23 + obj.InstanceType.GetHashCode();
                hash = hash*23 + obj.MemberName.GetHashCode();

                return hash;
            }
        }
    }
}
