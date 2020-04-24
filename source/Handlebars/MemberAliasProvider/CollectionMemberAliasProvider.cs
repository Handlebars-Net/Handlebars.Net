using System;
using System.Collections;
using System.Linq;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet
{
    internal class CollectionMemberAliasProvider : IMemberAliasProvider
    {
        private readonly InternalHandlebarsConfiguration _configuration;

        public CollectionMemberAliasProvider(InternalHandlebarsConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public bool TryGetMemberByAlias(object instance, Type targetType, string memberAlias, out object value)
        {
            var segment = ChainSegment.Create(memberAlias);
            switch (instance)
            {
                case Array array:
                    switch (segment.LowerInvariant)
                    {
                        case "count":
                            value = array.Length;
                            return true;
                        
                        default:
                            value = null;
                            return false;
                    }
                    
                case ICollection array:
                    switch (segment.LowerInvariant)
                    {
                        case "length":
                            value = array.Count;
                            return true;
                        
                        default:
                            value = null;
                            return false;
                    }
                   
                case IEnumerable enumerable:
                    if (!_configuration.ObjectDescriptorProvider.TryGetDescriptor(targetType, out var descriptor))
                    {
                        value = null;
                        return false;
                    }

                    var properties = descriptor.GetProperties(enumerable);
                    var property = properties.FirstOrDefault(o =>
                    {
                        var name = o.ToString().ToLowerInvariant();
                        return name.Equals("length") || name.Equals("count");
                    });

                    if (property != null && descriptor.MemberAccessor.TryGetValue(enumerable, targetType, property.ToString(), out value)) return true;
                    
                    value = null;
                    return false;

                default:
                    value = null;
                    return false;
            }
        }
    }
}