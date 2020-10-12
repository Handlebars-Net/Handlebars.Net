using System;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Allows to redirect member access to a different member
    /// </summary>
    public interface IMemberAliasProvider
    {
        bool TryGetMemberByAlias(object instance, Type targetType, string memberAlias, out object value);
    }
}