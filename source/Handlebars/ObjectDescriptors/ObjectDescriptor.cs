using System;
using System.Collections.Generic;
using HandlebarsDotNet.MemberAccessors;

namespace HandlebarsDotNet.ObjectDescriptors
{
    /// <summary>
    /// Provides meta-information about <see cref="Type"/>
    /// </summary>
    public class ObjectDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="describedType"></param>
        public ObjectDescriptor(Type describedType)
        {
            DescribedType = describedType;
        }

        /// <summary>
        /// Specifies whether the type should be treated as <see cref="System.Collections.IEnumerable"/>
        /// </summary>
        public bool ShouldEnumerate { get; set; }
        
        /// <summary>
        /// Returns type described by this instance of <see cref="ObjectDescriptor"/>
        /// </summary>
        public Type DescribedType { get; }
        
        /// <summary>
        /// Factory enabling receiving properties of specific instance   
        /// </summary>
        public Func<object, IEnumerable<object>> GetProperties { get; set; }
        
        /// <summary>
        /// <see cref="IMemberAccessor"/> associated with the <see cref="ObjectDescriptor"/>
        /// </summary>
        public IMemberAccessor MemberAccessor { get; set; }
    }
}