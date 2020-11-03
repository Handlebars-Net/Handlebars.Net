using System.Collections.Generic;

namespace HandlebarsDotNet.MemberAccessors.EnumerableAccessors
{
    public sealed class ReadOnlyListMemberAccessor<T, TV> : EnumerableMemberAccessor
        where T: IReadOnlyList<TV>
    {
        protected override bool TryGetValueInternal(object instance, int index, out object value)
        {
            var list = (T) instance;
            value = list[index];
            return true;
        }
    }
}