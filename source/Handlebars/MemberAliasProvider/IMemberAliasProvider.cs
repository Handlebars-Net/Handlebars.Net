using System;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Allows to redirect member access to a different member
    /// </summary>
    public interface IMemberAliasProvider
    {
        bool TryGetMemberByAlias(object instance, Type targetType, ChainSegment memberAlias, out object value);
    }
}