using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.EqualityComparers;
using HandlebarsDotNet.Iterators;
using HandlebarsDotNet.MemberAccessors.DictionaryAccessors;
using HandlebarsDotNet.Polyfills;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet.ObjectDescriptors
{
    public sealed class ReadOnlyGenericDictionaryObjectDescriptorProvider : IObjectDescriptorProvider
    {
        private static readonly MethodInfo CreateDescriptorMethodInfo = typeof(ReadOnlyGenericDictionaryObjectDescriptorProvider)
            .GetMethod(nameof(CreateDescriptor), BindingFlags.NonPublic | BindingFlags.Static);
        
        private readonly LookupSlim<Type, DeferredValue<Type, Type>, TypeEqualityComparer> _typeCache = new LookupSlim<Type, DeferredValue<Type, Type>, TypeEqualityComparer>(new TypeEqualityComparer());
        
        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            var interfaceType = _typeCache.GetOrAdd(type, InterfaceTypeValueFactory).Value;
            if (interfaceType == null)
            {
                value = ObjectDescriptor.Empty;
                return false;
            }
            
            var genericArguments = interfaceType.GetGenericArguments();

            var descriptorCreator = CreateDescriptorMethodInfo.MakeGenericMethod(type, genericArguments[0], genericArguments[1]);
                    
            value = (ObjectDescriptor) descriptorCreator.Invoke(null, ArrayEx.Empty<object>());
            return true;
        }

        private static readonly Func<Type, DeferredValue<Type, Type>> InterfaceTypeValueFactory =
            key =>
            {
                return new DeferredValue<Type, Type>(key, type =>
                {
                    return type.GetInterfaces()
                        .Where(i => IntrospectionExtensions.GetTypeInfo(i).IsGenericType)
                        .Where(i => i.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>))
                        .FirstOrDefault(i =>
                            TypeDescriptor.GetConverter(i.GetGenericArguments()[0]).CanConvertFrom(typeof(string))
                        );
                });
            };

        private static ObjectDescriptor CreateDescriptor<T, TK, TV>() 
            where T : class, IReadOnlyDictionary<TK, TV>
        {
            return new ObjectDescriptor(
                typeof(T),
                new ReadOnlyGenericDictionaryAccessor<T, TK, TV>(),
                (descriptor, o) => ((T) o).Keys,
                self => new ReadOnlyDictionaryIterator<T, TK, TV>()
            );
        }
    }
}