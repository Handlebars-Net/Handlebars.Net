using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.MemberAccessors.DictionaryAccessors;
using HandlebarsDotNet.Polyfills;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal sealed class StringDictionaryObjectDescriptorProvider : IObjectDescriptorProvider
    {
        private static readonly object[] EmptyArray = ArrayEx.Empty<object>();
        private static readonly MethodInfo CreateClassDescriptorMethodInfo = typeof(StringDictionaryObjectDescriptorProvider)
            .GetMethod(nameof(CreateClassDescriptor), BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo CreateStructDescriptorMethodInfo = typeof(StringDictionaryObjectDescriptorProvider)
            .GetMethod(nameof(CreateStructDescriptor), BindingFlags.NonPublic | BindingFlags.Static);
        
        private readonly LookupSlim<Type, DeferredValue<Type, Type>> _typeCache = new LookupSlim<Type, DeferredValue<Type, Type>>();

        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            var interfaceType = _typeCache.GetOrAdd(type, InterfaceTypeValueFactory).Value;
            if (interfaceType == null)
            {
                value = ObjectDescriptor.Empty;
                return false;
            }

            var typeArgument = interfaceType.GetGenericArguments()[1];
            var factory = typeArgument.GetTypeInfo().IsClass
                ? CreateClassDescriptorMethodInfo
                : CreateStructDescriptorMethodInfo;
            
            var descriptorCreator = factory
                .MakeGenericMethod(type, typeArgument);

            value = (ObjectDescriptor) descriptorCreator.Invoke(null, EmptyArray);
            return true;
        }
        
        private static readonly Func<Type, DeferredValue<Type, Type>> InterfaceTypeValueFactory = 
            key => new DeferredValue<Type, Type>(key, type =>
            {
                return type.GetInterfaces()
                    .FirstOrDefault(i =>
                        i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>) &&
                        i.GetGenericArguments()[0] == typeof(string));
            });

        private static ObjectDescriptor CreateClassDescriptor<T, TV>() 
            where T : IDictionary<string, TV>
            where TV: class
        {
            return new ObjectDescriptor(
                typeof(IDictionary<string, TV>),
                new StringClassDictionaryAccessor<T, TV>(),
                (descriptor, o) => ((T) o).Keys
            );
        }
        
        private static ObjectDescriptor CreateStructDescriptor<T, TV>()
            where T : IDictionary<string, TV>
            where TV: struct
        {
            return new ObjectDescriptor(
                typeof(IDictionary<string, TV>),
                new StringStructDictionaryAccessor<T, TV>(),
                (descriptor, o) => ((T) o).Keys
            );
        }
    }
}