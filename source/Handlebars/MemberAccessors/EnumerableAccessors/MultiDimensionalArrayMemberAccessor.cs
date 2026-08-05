using System;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet.MemberAccessors.EnumerableAccessors
{
    public sealed class MultiDimensionalArrayMemberAccessor : EnumerableMemberAccessor
    {
        protected override bool TryGetValueInternal(object instance, int index, out object? value)
        {
            var array = (Array) instance;
            if (index >= array.GetLength(0))
            {
                value = null;
                return false;
            }

            value = new MultidimensionalArraySlice(array, new[] { index });
            return true;
        }
    }
}
