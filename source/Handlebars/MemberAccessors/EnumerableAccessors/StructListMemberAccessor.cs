using System.Collections.Generic;

namespace HandlebarsDotNet.MemberAccessors.EnumerableAccessors
{
    internal sealed class StructListMemberAccessor<T, TV> : EnumerableMemberAccessor
        where T: IList<TV>
        where TV: struct
    {
        private static readonly object BoxedDefault = default(TV);

        protected override bool TryGetValueInternal(object instance, int index, out object value)
        {
            var list = (T) instance;
            if (index >= list.Count)
            {
                value = BoxedDefault;
                return false;
            }
            
            value = list[index];
            return true;
        }
    }
}