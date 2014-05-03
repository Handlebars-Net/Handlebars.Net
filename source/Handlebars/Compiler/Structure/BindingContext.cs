using System;
using System.IO;

namespace Handlebars.Compiler
{
    internal class BindingContext
    {
        private readonly object _value;
        private readonly BindingContext _parent;
        private readonly TextWriter _writer;

        public BindingContext(object value, TextWriter writer, BindingContext parent)
        {
            _value = value;
            _writer = writer;
            _parent = parent;
        }

        public object Value
        {
            get { return _value; }
        }

        public BindingContext ParentContext
        {
            get { return _parent; }
        }

        public TextWriter TextWriter
        {
            get { return _writer; }
        }
    }
}

