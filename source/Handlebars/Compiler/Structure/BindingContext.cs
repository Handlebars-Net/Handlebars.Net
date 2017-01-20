﻿using System.Reflection;

namespace HandlebarsDotNet.Compiler
{
    internal class BindingContext
    {
        private readonly object _value;
        private readonly BindingContext _parent;

        public string TemplatePath { get; private set; }

        public EncodedTextWriter TextWriter { get; private set; }

        public bool SuppressEncoding
        {
            get { return TextWriter.SuppressEncoding; }
            set { TextWriter.SuppressEncoding = value; }
        }

        public BindingContext(object value, EncodedTextWriter writer, BindingContext parent, string templatePath)
        {
            TemplatePath = parent != null ? (parent.TemplatePath ?? templatePath) : templatePath;
            TextWriter = writer;
            _value = value;
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

        public virtual object Root
        {
            get
            {
                var currentContext = this;
                while (currentContext.ParentContext != null)
                {
                    currentContext = currentContext.ParentContext;
                }
                return currentContext.Value;
            }
        }

        public virtual object GetContextVariable(string variableName)
        {
            var target = this;

            return GetContextVariable(variableName, target)
                   ?? GetContextVariable(variableName, target.Value);
        }

        private object GetContextVariable(string variableName, object target)
        {
            object returnValue;
            variableName = variableName.TrimStart('@');
            var member = target.GetType().GetMember(variableName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (member.Length > 0)
            {
                if (member[0] is PropertyInfo)
                {
                    returnValue = ((PropertyInfo)member[0]).GetValue(target, null);
                }
                else if (member[0] is FieldInfo)
                {
                    returnValue = ((FieldInfo)member[0]).GetValue(target);
                }
                else
                {
                    throw new HandlebarsRuntimeException("Context variable references a member that is not a field or property");
                }
            }
            else if (_parent != null)
            {
                returnValue = _parent.GetContextVariable(variableName);
            }
            else
            {
                returnValue = null;
            }
            return returnValue;
        }

        public virtual BindingContext CreateChildContext(object value)
        {
            return new BindingContext(value, TextWriter, this, TemplatePath);
        }
    }
}

