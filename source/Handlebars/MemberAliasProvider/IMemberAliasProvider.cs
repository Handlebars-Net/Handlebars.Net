using System;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Allows to redirect member access to a different member
    /// </summary>
    public interface IMemberAliasProvider<in T>
    {
        bool TryGetMemberByAlias(T instance, Type targetType, ChainSegment memberAlias, out object value);
    }
    
    /// <summary>
    /// Allows to redirect member access to a different member
    /// </summary>
    public interface IMemberAliasProvider : IMemberAliasProvider<object>
    {
    }
}