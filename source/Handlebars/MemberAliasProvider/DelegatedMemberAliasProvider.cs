using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Provides simple interface for adding member aliases
    /// </summary>
    public class DelegatedMemberAliasProvider : IMemberAliasProvider
    {
        private readonly Dictionary<Type, Dictionary<string, Func<object, object>>> _aliases 
            = new Dictionary<Type, Dictionary<string, Func<object, object>>>();
        
        public DelegatedMemberAliasProvider AddAlias(Type type, string alias, Func<object, object> accessor)
        {
            if (!_aliases.TryGetValue(type, out var aliases))
            {
                aliases = new Dictionary<string, Func<object, object>>(StringComparer.OrdinalIgnoreCase);
                _aliases.Add(type, aliases);
            }
            
            aliases.Add(alias, accessor);
            
            return this;
        }
        
        public DelegatedMemberAliasProvider AddAlias<T>(string alias, Func<T, object> accessor)
        {
            AddAlias(typeof(T), alias, o => accessor((T) o));
            
            return this;
        }
        
        bool IMemberAliasProvider.TryGetMemberByAlias(object instance, Type targetType, ChainSegment memberAlias,
            out object value)
        {
            if (_aliases.TryGetValue(targetType, out var aliases))
            {
                if (aliases.TryGetValue(memberAlias, out var accessor))
                {
                    value = accessor(instance);
                    return true;
                }
            }
            
            aliases = _aliases.FirstOrDefault(o => o.Key.IsAssignableFrom(targetType)).Value;
            if (aliases != null)
            {
                if (aliases.TryGetValue(memberAlias, out var accessor))
                {
                    value = accessor(instance);
                    return true;
                }
            }

            value = null;
            return false;
        }
    }
}