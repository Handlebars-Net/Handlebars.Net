using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.ObjectDescriptors;

namespace HandlebarsDotNet
{
    public readonly ref struct ObjectAccessor
    {
        private readonly object _data;
        private readonly ObjectDescriptor _descriptor;

        public ObjectAccessor(object data, ObjectDescriptor descriptor)
        {
            _data = data;
            _descriptor = descriptor;
        }

        public IEnumerable<ChainSegment> Properties => _descriptor
            .GetProperties(_descriptor, _data)
            .OfType<object>()
            .Select(ChainSegment.Create);

        public object this[ChainSegment segment] =>
            _descriptor.MemberAccessor.TryGetValue(_data, segment, out var value) 
                ? value 
                : null;
    }
}