using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.MemberAccessors;
using HandlebarsDotNet.ObjectDescriptors;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet
{
    public readonly ref struct ObjectAccessor
    {
        private readonly object _data;
        private readonly ObjectDescriptor _descriptor;
        private readonly IMemberAccessor _memberAccessor;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectAccessor(object data, ObjectDescriptor descriptor)
        {
            _data = data;
            _descriptor = descriptor;
            _memberAccessor = _descriptor.MemberAccessor;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectAccessor(object data)
        {
            _data = data;
            if (data == null || !ObjectDescriptorFactory.Current.TryGetDescriptor(data.GetType(), out _descriptor))
            {
                _descriptor = ObjectDescriptor.Empty;
                _memberAccessor = null;
            }
            else
            {
                _memberAccessor = _descriptor.MemberAccessor;   
            }
        }

        public IEnumerable<ChainSegment> Properties => _descriptor
            .GetProperties(_descriptor, _data)
            .OfType<object>()
            .Select(ChainSegment.Create);

        public object this[ChainSegment segment] =>
            _memberAccessor != null && _memberAccessor.TryGetValue(_data, segment, out var value)
                ? value
                : null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(ChainSegment segment, out object value)
        {
            if (_memberAccessor == null)
            {
                value = null;
                return false;
            }
            
            return _memberAccessor.TryGetValue(_data, segment, out value);
        }
    }
}