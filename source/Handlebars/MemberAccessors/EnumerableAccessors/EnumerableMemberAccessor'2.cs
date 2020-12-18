using System.Collections.Generic;

namespace HandlebarsDotNet.MemberAccessors.EnumerableAccessors
{
    public sealed class EnumerableMemberAccessor<T, TV> : EnumerableMemberAccessor
        where T: class, IEnumerable<TV>
    {
        protected override bool TryGetValueInternal(object instance, int index, out object value)
        {
            var list = (T) instance;
            using var e = list.GetEnumerator();
            while (e.MoveNext())
            {
                if (index-- != 0) continue;
                        
                value = e.Current;
                return true;
            }

            value = null;
            return false;
        }
    }
}