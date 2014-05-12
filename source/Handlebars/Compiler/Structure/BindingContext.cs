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

        public virtual object Value
        {
            get { return _value; }
        }

        public virtual BindingContext ParentContext
        {
            get { return _parent; }
        }

        public virtual TextWriter TextWriter
        {
            get { return _writer; }
        }

        public virtual object GetContextVariable(string variableName)
        {
            object returnValue = null;
            var parent = _parent;
            while (returnValue == null && parent != null)
            {
                returnValue = parent.GetContextVariable(variableName);
                parent = parent.ParentContext;
            }
            return returnValue;
        }
    }
}

