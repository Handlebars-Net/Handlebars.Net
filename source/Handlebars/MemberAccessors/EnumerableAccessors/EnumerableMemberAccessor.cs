using System;
using System.Collections;
using System.Collections.Generic;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.MemberAccessors.EnumerableAccessors
{
    public class EnumerableMemberAccessor : IMemberAccessor
    {
        public static EnumerableMemberAccessor Create(Type type)
        {
            if (type.IsAssignableToGenericType(typeof(IList<>), out var genericType))
            {
                var typeArgument = genericType.GenericTypeArguments[0];
                var accessorType = typeof(ListMemberAccessor<,>).MakeGenericType(genericType, typeArgument);

                return (EnumerableMemberAccessor) Activator.CreateInstance(accessorType);
            }
            
            if (type.IsAssignableToGenericType(typeof(IReadOnlyList<>), out genericType))
            {
                var typeArgument = genericType.GenericTypeArguments[0];
                var accessorType = typeof(ReadOnlyListMemberAccessor<,>).MakeGenericType(genericType, typeArgument);

                return (EnumerableMemberAccessor) Activator.CreateInstance(accessorType);
            }
            
            if (type.IsAssignableToGenericType(typeof(IEnumerable<>), out genericType))
            {
                var typeArgument = genericType.GenericTypeArguments[0];
                var accessorType = typeof(EnumerableMemberAccessor<,>).MakeGenericType(genericType, typeArgument);

                return (EnumerableMemberAccessor) Activator.CreateInstance(accessorType);
            }
            
            return new EnumerableMemberAccessor();
        }
        
        protected EnumerableMemberAccessor() { }
        
        public virtual bool TryGetValue(object instance, ChainSegment memberName, out object value)
        {
            if (int.TryParse(memberName.LowerInvariant, out var index) && index >= 0) 
                return TryGetValueInternal(instance, index, out value);
            
            value = null;
            return false;
        }

        protected virtual bool TryGetValueInternal(object instance, int index, out object value)
        {
            switch (instance)
            {
                case IList list when list.Count > index:
                    value = list[index];
                    return true;

                case IEnumerable enumerable:
                    var e = enumerable.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (index-- != 0) continue;
                        
                        value = e.Current;
                        return true;
                    }

                    value = null;
                    return false;
            }

            value = null;
            return false;
        }
    }
}