using System;

namespace HandlebarsDotNet.ObjectDescriptors
{
    /// <summary>
    /// Facade for <see cref="ObjectDescriptor"/>
    /// </summary>
    public interface IObjectDescriptorProvider
    {
        /// <summary>
        /// Tries to create <see cref="ObjectDescriptor"/> for <paramref name="type"/>. 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        bool TryGetDescriptor(Type type, out ObjectDescriptor value);
    }
}