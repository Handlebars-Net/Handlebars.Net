using System;
using System.IO;
using System.Reflection;

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
            object returnValue;
            variableName = variableName.Trim('@');
            var member = this.GetType().GetMember(variableName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if(member.Length > 0)
            {
                if(member[0].MemberType == MemberTypes.Property)
                {
                    returnValue = ((PropertyInfo)member[0]).GetValue(this, BindingFlags.Default, null, null, null);
                }
                else if(member[0].MemberType == MemberTypes.Field)
                {
                    returnValue = ((FieldInfo)member[0]).GetValue(this);
                }
                else
                {
                    throw new HandlebarsRuntimeException("Context variable references a member that is not a field or property");
                }
            }
            else if(_parent != null)
            {
                returnValue = _parent.GetContextVariable(variableName);
            }
            else
            {
                returnValue = null;
            }
            return returnValue;
        }
    }
}

