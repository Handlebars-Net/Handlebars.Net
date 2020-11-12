using System;
using System.Collections;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.Iterators;
using HandlebarsDotNet.MemberAccessors;

namespace HandlebarsDotNet.ObjectDescriptors
{
    /// <summary>
    /// Provides meta-information about <see cref="Type"/>
    /// </summary>
    public sealed class ObjectDescriptor
    {
        public static readonly ObjectDescriptor Empty = new ObjectDescriptor();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ObjectDescriptor Create(object from, IObjectDescriptorProvider objectDescriptorProvider)
        {
            return Create(from?.GetType(), objectDescriptorProvider);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ObjectDescriptor Create(Type from, IObjectDescriptorProvider objectDescriptorProvider)
        {
            if (from == null) return null;
            if (!objectDescriptorProvider.TryGetDescriptor(@from, out var descriptor)) return null;
            return descriptor;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreate(object @from, IObjectDescriptorProvider objectDescriptorProvider, out ObjectDescriptor descriptor)
        {
            return TryCreate(from?.GetType(), objectDescriptorProvider, out descriptor);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreate(Type @from, IObjectDescriptorProvider objectDescriptorProvider, out ObjectDescriptor descriptor)
        {
            return objectDescriptorProvider.TryGetDescriptor(from, out descriptor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="describedType">Returns type described by this instance of <see cref="ObjectDescriptor"/></param>
        /// <param name="memberAccessor"><see cref="IMemberAccessor"/> associated with the <see cref="ObjectDescriptor"/></param>
        /// <param name="getProperties">Factory enabling receiving properties of specific instance</param>
        /// <param name="iterator"></param>
        /// <param name="dependencies"></param>
        public ObjectDescriptor(
            Type describedType, 
            IMemberAccessor memberAccessor,
            Func<ObjectDescriptor, object, IEnumerable> getProperties,
            Func<ObjectDescriptor, IIterator> iterator,
            params object[] dependencies
        )
        {
            DescribedType = describedType;
            GetProperties = getProperties;
            MemberAccessor = memberAccessor;
            Dependencies = dependencies;
            Iterator = iterator(this);
        }
        
        private ObjectDescriptor(){ }

        /// <summary>
        /// Iterator implementation for <see cref="DescribedType"/>
        /// </summary>
        public readonly IIterator Iterator;
        
        /// <summary>
        /// Contains dependencies for <see cref="GetProperties"/> delegate
        /// </summary>
        public readonly object[] Dependencies;

        /// <summary>
        /// Returns type described by this instance of <see cref="ObjectDescriptor"/>
        /// </summary>
        public readonly Type DescribedType;

        /// <summary>
        /// Factory enabling receiving properties of specific instance   
        /// </summary>
        public readonly Func<ObjectDescriptor, object, IEnumerable> GetProperties;

        /// <summary>
        /// <see cref="IMemberAccessor"/> associated with the <see cref="ObjectDescriptor"/>
        /// </summary>
        public readonly IMemberAccessor MemberAccessor;
    }
}