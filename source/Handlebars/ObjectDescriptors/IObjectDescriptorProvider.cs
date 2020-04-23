using System;

namespace HandlebarsDotNet.ObjectDescriptors
{
    /// <summary>
    /// Factory for <see cref="ObjectDescriptor"/>
    /// </summary>
    public interface IObjectDescriptorProvider
    {
        /// <summary>
        /// Lightweight method to check whether descriptor can be created
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool CanHandleType(Type type);
        
        /// <summary>
        /// Tries to create <see cref="ObjectDescriptor"/> for <paramref name="type"/>. Methods is guarantied to be called if <see cref="CanHandleType"/> return <see langword="true"/>. 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        bool TryGetDescriptor(Type type, out ObjectDescriptor value);
    }
}