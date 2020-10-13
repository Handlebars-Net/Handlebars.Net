using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.MemberAccessors.DictionaryAccessors;
using HandlebarsDotNet.Polyfills;

namespace HandlebarsDotNet.ObjectDescriptors
{
    internal sealed class GenericDictionaryObjectDescriptorProvider : IObjectDescriptorProvider
    {
        private static readonly MethodInfo CreateClassDescriptorWithClassPropertiesMethodInfo = typeof(GenericDictionaryObjectDescriptorProvider)
            .GetMethod(nameof(CreateClassDescriptorWithClassProperties), BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo CreateClassDescriptorWithStructPropertiesMethodInfo = typeof(GenericDictionaryObjectDescriptorProvider)
            .GetMethod(nameof(CreateClassDescriptorWithStructProperties), BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo CreateStructDescriptorWithClassPropertiesMethodInfo = typeof(GenericDictionaryObjectDescriptorProvider)
            .GetMethod(nameof(CreateStructDescriptorWithClassProperties), BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo CreateStructDescriptorWithStructPropertiesMethodInfo = typeof(GenericDictionaryObjectDescriptorProvider)
            .GetMethod(nameof(CreateStructDescriptorWithStructProperties), BindingFlags.NonPublic | BindingFlags.Static);
        
        
        private readonly LookupSlim<Type, DeferredValue<Type, Type>> _typeCache = new LookupSlim<Type, DeferredValue<Type, Type>>();
        
        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            var interfaceType = _typeCache.GetOrAdd(type, InterfaceTypeValueFactory).Value;
            if (interfaceType == null)
            {
                value = ObjectDescriptor.Empty;
                return false;
            }
            
            var genericArguments = interfaceType.GetGenericArguments();
            var factory = genericArguments[1].GetTypeInfo().IsClass
                ? genericArguments[0].GetTypeInfo().IsClass
                    ? CreateClassDescriptorWithClassPropertiesMethodInfo
                    : CreateClassDescriptorWithStructPropertiesMethodInfo
                : genericArguments[0].GetTypeInfo().IsClass
                    ? CreateStructDescriptorWithClassPropertiesMethodInfo
                    : CreateStructDescriptorWithStructPropertiesMethodInfo;
            
            var descriptorCreator = factory.MakeGenericMethod(type, genericArguments[0], genericArguments[1]);
                    
            value = (ObjectDescriptor) descriptorCreator.Invoke(null, ArrayEx.Empty<object>());
            return true;
        }

        private static readonly Func<Type, DeferredValue<Type, Type>> InterfaceTypeValueFactory =
            key => new DeferredValue<Type, Type>(key, type =>
            {
                return type.GetInterfaces()
                    .Where(i => i.GetTypeInfo().IsGenericType)
                    .Where(i => i.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                    .FirstOrDefault(i =>
                        TypeDescriptor.GetConverter(i.GetGenericArguments()[0]).CanConvertFrom(typeof(string))
                    );
            });

        private static ObjectDescriptor CreateClassDescriptorWithClassProperties<T, TK, TV>() 
            where T : IDictionary<TK, TV>
            where TK: class
            where TV: class
        {
            return new ObjectDescriptor(
                typeof(IDictionary<TK, TV>), 
                new GenericClassDictionaryAccessor<T, TK, TV>(),
                (descriptor, o) => ((IDictionary<TK, TV>) o).Keys
            );
        }
        
        private static ObjectDescriptor CreateClassDescriptorWithStructProperties<T, TK, TV>() 
            where T : IDictionary<TK, TV>
            where TK: struct
            where TV: class
        {
            return new ObjectDescriptor(
                typeof(IDictionary<TK, TV>), 
                new GenericClassDictionaryAccessor<T, TK, TV>(),
                (descriptor, o) => ((IDictionary<TK, TV>) o).Keys
            );
        }
        
        private static ObjectDescriptor CreateStructDescriptorWithClassProperties<T, TK, TV>() 
            where T : IDictionary<TK, TV>
            where TK: class
            where TV: struct
        {
            return new ObjectDescriptor(
                typeof(IDictionary<TK, TV>), 
                new GenericStructDictionaryAccessor<T, TK, TV>(),
                (descriptor, o) => ((IDictionary<TK, TV>) o).Keys
            );
        }
        
        private static ObjectDescriptor CreateStructDescriptorWithStructProperties<T, TK, TV>() 
            where T : IDictionary<TK, TV>
            where TK: struct
            where TV: struct
        {
            return new ObjectDescriptor(
                typeof(IDictionary<TK, TV>), 
                new GenericStructDictionaryAccessor<T, TK, TV>(),
                (descriptor, o) => ((IDictionary<TK, TV>) o).Keys
            );
        }
    }
}