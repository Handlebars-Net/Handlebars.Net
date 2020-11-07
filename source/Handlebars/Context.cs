using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.ObjectDescriptors;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet
{
    public readonly struct Context
    {
        private readonly DeferredValue<BindingContext, ObjectDescriptor> _descriptor;
        
        public readonly object Value;

        public Context(BindingContext context)
        {
            Value = context.Value;
            _descriptor = context.ObjectDescriptor;
        }
        
        public Context(BindingContext context, object value)
        {
            Value = value;
            _descriptor = context.ObjectDescriptor;
        }

        public IEnumerable<ChainSegment> Properties => 
            _descriptor.Value
                .GetProperties(_descriptor.Value, Value)
                .OfType<object>()
                .Select(ChainSegment.Create);

        public object this[ChainSegment segment] =>
            _descriptor.Value.MemberAccessor.TryGetValue(Value, segment, out var value) 
                ? value 
                : null;

        public T GetValue<T>(ChainSegment segment)
        {
            if (!_descriptor.Value.MemberAccessor.TryGetValue(Value, segment, out var obj)) return default;
            if (obj is T value) return value;

            var converter = TypeDescriptor.GetConverter(obj.GetType());
            return (T) converter.ConvertTo(obj, typeof(T));
        }
    }
}