using System;
using System.Collections;
using System.Linq;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Iterators;
using HandlebarsDotNet.MemberAccessors;
using HandlebarsDotNet.ObjectDescriptors;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet
{
    internal class LayoutViewModel
    {
        private static readonly ChainSegment _bodyChainSegment = ChainSegment.Create("body");
        private readonly string _body;
        private readonly object _value;
        private readonly ObjectDescriptor _valueDescriptor;

        public LayoutViewModel(string body, object value)
        {
            _body = body;
            _value = value;
            _valueDescriptor = ObjectDescriptor.Create(value);
        }

        internal class DescriptorProvider: IObjectDescriptorProvider
        {
            private static readonly object[] _bodyProperties = { _bodyChainSegment };
            private static readonly Type _type = typeof(LayoutViewModel);
            private readonly ObjectDescriptor _descriptor;

            public DescriptorProvider()
            {
                _descriptor = new ObjectDescriptor(
                    _type,
                    new MemberAccessor(),
                    (_, o) =>
                    {
                        var vm = (LayoutViewModel) o;
                        IEnumerable valueProperties = vm._valueDescriptor.GetProperties(vm._valueDescriptor, vm._value);

                        return _bodyProperties
                           .Concat(valueProperties.Cast<object>());
                    },
                    _ => new Iterator()
                );
            }

            public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
            {
                if (type != _type)
                {
                    value = ObjectDescriptor.Empty;
                    return false;
                }

                value = _descriptor;
                return true;
            }
        }

        private class MemberAccessor: IMemberAccessor
        {
            public bool TryGetValue(object instance, ChainSegment memberName, out object value)
            {
                var vm = (LayoutViewModel) instance;

                if (memberName.Equals(_bodyChainSegment))
                {
                    value = vm._body;
                    return true;
                }

                var memberAccessor = vm._valueDescriptor.MemberAccessor;

                if (memberAccessor != null)
                    return memberAccessor.TryGetValue(vm._value, memberName, out value);

                value = default;
                return false;
            }
        }

        private class Iterator: IIterator
        {
            public void Iterate(in EncodedTextWriter writer, BindingContext context, ChainSegment[] blockParamsVariables, object input, TemplateDelegate template, TemplateDelegate ifEmpty)
            {
                var vm = (LayoutViewModel) input;
                var iterator = vm._valueDescriptor.Iterator;
                iterator?.Iterate(writer, context, blockParamsVariables, vm._value, template, ifEmpty);
            }
        }
    }
}