using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HandlebarsDotNet.Iterators;
using HandlebarsDotNet.MemberAccessors;
using HandlebarsDotNet.MemberAccessors.EnumerableAccessors;

namespace HandlebarsDotNet.ObjectDescriptors
{
    public sealed class EnumerableObjectDescriptor : IObjectDescriptorProvider
    {
        private static readonly Type Type = typeof(IEnumerable);
        private static readonly Type StringType = typeof(string);
        private static readonly Type EnumerableObjectDescriptorType = typeof(EnumerableObjectDescriptor);
        private static readonly BindingFlags BindingFlags = BindingFlags.NonPublic | BindingFlags.Static;

        private static readonly MethodInfo ArrayObjectDescriptorFactoryMethodInfo = EnumerableObjectDescriptorType
            .GetMethod(nameof(ArrayObjectDescriptorFactory), BindingFlags);
        
        private static readonly MethodInfo ListObjectDescriptorFactoryMethodInfo = EnumerableObjectDescriptorType
            .GetMethod(nameof(ListObjectDescriptorFactory), BindingFlags);
        
        private static readonly MethodInfo ReadOnlyListObjectDescriptorFactoryMethodInfo = EnumerableObjectDescriptorType
            .GetMethod(nameof(ReadOnlyListObjectDescriptorFactory), BindingFlags);
        
        private static readonly MethodInfo NonGenericListObjectDescriptorFactoryMethodInfo = EnumerableObjectDescriptorType
            .GetMethod(nameof(NonGenericListObjectDescriptorFactory), BindingFlags);
        
        private static readonly MethodInfo CollectionObjectDescriptorFactoryMethodInfo = EnumerableObjectDescriptorType
            .GetMethod(nameof(CollectionObjectDescriptorFactory), BindingFlags);
        
        private static readonly MethodInfo ReadOnlyCollectionObjectDescriptorFactoryMethodInfo = EnumerableObjectDescriptorType
            .GetMethod(nameof(ReadOnlyCollectionObjectDescriptorFactory), BindingFlags);
        
        private static readonly MethodInfo NonGenericCollectionObjectDescriptorFactoryMethodInfo = EnumerableObjectDescriptorType
            .GetMethod(nameof(NonGenericCollectionObjectDescriptorFactory), BindingFlags);
        
        private static readonly MethodInfo EnumerableObjectDescriptorFactoryMethodInfo = EnumerableObjectDescriptorType
            .GetMethod(nameof(EnumerableObjectDescriptorFactory), BindingFlags);
        
        private static readonly MethodInfo NonGenericEnumerableObjectDescriptorFactoryMethodInfo = EnumerableObjectDescriptorType
            .GetMethod(nameof(NonGenericEnumerableObjectDescriptorFactory), BindingFlags);

        private readonly ObjectDescriptorProvider _descriptorProvider;

        public EnumerableObjectDescriptor(ObjectDescriptorProvider descriptorProvider)
        {
            _descriptorProvider = descriptorProvider;
        }
        
        public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
        {
            if (!(type != StringType && Type.IsAssignableFrom(type)))
            {
                value = ObjectDescriptor.Empty;
                return false;
            }
            
            if (!_descriptorProvider.TryGetDescriptor(type, out value))
            {
                value = ObjectDescriptor.Empty;
                return false;
            }
            
            var enumerableMemberAccessor = EnumerableMemberAccessor.Create(type);
            var mergedMemberAccessor = new MergedMemberAccessor(enumerableMemberAccessor, value.MemberAccessor);

            var parameters = new object[]{ mergedMemberAccessor, value };
            return TryCreateArrayDescriptor(type, parameters, out value)
                   || TryCreateDescriptorFromOpenGeneric(type, typeof(IList<>), parameters, ListObjectDescriptorFactoryMethodInfo, out value)
                   || TryCreateDescriptorFromOpenGeneric(type, typeof(IReadOnlyList<>), parameters, ReadOnlyListObjectDescriptorFactoryMethodInfo, out value)
                   || TryCreateDescriptorFromOpenGeneric(type, typeof(ICollection<>), parameters, CollectionObjectDescriptorFactoryMethodInfo, out value)
                   || TryCreateDescriptorFromOpenGeneric(type, typeof(IReadOnlyCollection<>), parameters, ReadOnlyCollectionObjectDescriptorFactoryMethodInfo, out value)
                   || TryCreateDescriptorFromOpenGeneric(type, typeof(IEnumerable<>), parameters, EnumerableObjectDescriptorFactoryMethodInfo, out value)
                   || TryCreateDescriptor(type, typeof(IList), parameters, NonGenericListObjectDescriptorFactoryMethodInfo, out value)
                   || TryCreateDescriptor(type, typeof(ICollection), parameters, NonGenericCollectionObjectDescriptorFactoryMethodInfo, out value)
                   || TryCreateDescriptor(type, typeof(IEnumerable), parameters, NonGenericEnumerableObjectDescriptorFactoryMethodInfo, out value);
        }

        private static bool TryCreateArrayDescriptor(Type type, object[] parameters, out ObjectDescriptor value)
        {
            if (type.IsArray)
            {
                value = (ObjectDescriptor) ArrayObjectDescriptorFactoryMethodInfo
                    .MakeGenericMethod(type.GetElementType())
                    .Invoke(null, parameters);

                return true;
            }

            value = ObjectDescriptor.Empty;
            return false;
        }

        private static bool TryCreateDescriptorFromOpenGeneric(Type type, Type openGenericType, object[] parameters, MethodInfo method, out ObjectDescriptor descriptor)
        {
            if (type.IsAssignableToGenericType(openGenericType, out var genericType))
            {
                descriptor = (ObjectDescriptor) method
                    .MakeGenericMethod(type, genericType.GenericTypeArguments[0])
                    .Invoke(null, parameters);

                return true;
            }

            descriptor = ObjectDescriptor.Empty;
            return false;
        }
        
        private static bool TryCreateDescriptor(Type type, Type targetType, object[] parameters, MethodInfo method, out ObjectDescriptor descriptor)
        {
            if (targetType.IsAssignableFrom(type))
            {
                descriptor = (ObjectDescriptor) method
                    .MakeGenericMethod(type)
                    .Invoke(null, parameters);

                return true;
            }

            descriptor = ObjectDescriptor.Empty;
            return false;
        }

        private static ObjectDescriptor ArrayObjectDescriptorFactory<TValue>(IMemberAccessor accessor, ObjectDescriptor descriptor)
        {
            return new ObjectDescriptor(typeof(TValue[]), accessor, descriptor.GetProperties, self => new ArrayIterator<TValue>(), descriptor.Dependencies);
        }
        
        private static ObjectDescriptor ListObjectDescriptorFactory<T, TValue>(IMemberAccessor accessor, ObjectDescriptor descriptor) 
            where T : IList<TValue>
        {
            return new ObjectDescriptor(typeof(T), accessor, descriptor.GetProperties, self => new ListIterator<T, TValue>(), descriptor.Dependencies);
        }
        
        private static ObjectDescriptor ReadOnlyListObjectDescriptorFactory<T, TValue>(IMemberAccessor accessor, ObjectDescriptor descriptor) 
            where T : IReadOnlyList<TValue>
        {
            return new ObjectDescriptor(typeof(T), accessor, descriptor.GetProperties, self => new ReadOnlyListIterator<T, TValue>(), descriptor.Dependencies);
        }
        
        private static ObjectDescriptor NonGenericListObjectDescriptorFactory<T>(IMemberAccessor accessor, ObjectDescriptor descriptor) 
            where T : IList
        {
            return new ObjectDescriptor(typeof(T), accessor, descriptor.GetProperties, self => new ListIterator<T>(), descriptor.Dependencies);
        }
        
        private static ObjectDescriptor CollectionObjectDescriptorFactory<T, TValue>(IMemberAccessor accessor, ObjectDescriptor descriptor) 
            where T : ICollection<TValue>
        {
            return new ObjectDescriptor(typeof(T), accessor, descriptor.GetProperties, self => new CollectionIterator<T, TValue>(), descriptor.Dependencies);
        }
        
        private static ObjectDescriptor ReadOnlyCollectionObjectDescriptorFactory<T, TValue>(IMemberAccessor accessor, ObjectDescriptor descriptor) 
            where T : IReadOnlyCollection<TValue>
        {
            return new ObjectDescriptor(typeof(T), accessor, descriptor.GetProperties, self => new ReadOnlyCollectionIterator<T, TValue>(), descriptor.Dependencies);
        }
        
        private static ObjectDescriptor NonGenericCollectionObjectDescriptorFactory<T>(IMemberAccessor accessor, ObjectDescriptor descriptor) 
            where T : ICollection
        {
            return new ObjectDescriptor(typeof(T), accessor, descriptor.GetProperties, self => new CollectionIterator<T>(), descriptor.Dependencies);
        }
        
        private static ObjectDescriptor EnumerableObjectDescriptorFactory<T, TValue>(IMemberAccessor accessor, ObjectDescriptor descriptor) 
            where T : IEnumerable<TValue>
        {
            return new ObjectDescriptor(typeof(T), accessor, descriptor.GetProperties, self => new EnumerableIterator<T, TValue>(), descriptor.Dependencies);
        }
        
        private static ObjectDescriptor NonGenericEnumerableObjectDescriptorFactory<T>(IMemberAccessor accessor, ObjectDescriptor descriptor) 
            where T : IEnumerable
        {
            return new ObjectDescriptor(typeof(T), accessor, descriptor.GetProperties, self => new EnumerableIterator<T>(), descriptor.Dependencies);
        }
    }
}