using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.ObjectDescriptors;

namespace HandlebarsDotNet.MemberAccessors
{
    /// <summary>
    /// Describes mechanism to access members of object
    /// </summary>
    public interface IMemberAccessor
    {
        /// <summary>
        /// Describes mechanism to access members of an object. Returns <see langword="true"/> if operation is successful and <paramref name="value"/> contains data, otherwise returns <see langword="false"/> 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGetValue(object instance, ChainSegment memberName, out object value);
    }

    public readonly ref struct MemberAccessor
    {
        private readonly object _instance;
        private readonly IMemberAccessor _accessor;

        public MemberAccessor(object instance, ObjectDescriptor descriptor)
        {
            _instance = instance;
            _accessor = descriptor.MemberAccessor;
        }
        
        public object this[ChainSegment segment]
        {
            get
            {
                if (_accessor.TryGetValue(_instance, segment, out var value))
                {
                    return value;
                }

                return null;
            }
        }
    }
}