using System.IO;
using System.Reflection;

namespace HandlebarsDotNet.Compiler
{
    internal class BindingContext
    {
        private readonly object _value;
        private readonly BindingContext _parent;
        private readonly TextWriter _encodedWriter;
        private readonly TextWriter _unencodedWriter;
        public string TemplatePath { get; private set; }

        public ITextEncoder TextEncoder { get; private set; }

        public BindingContext(object value, TextWriter writer, BindingContext parent, string templatePath, ITextEncoder textEncoder)
        {
            TemplatePath = (parent == null ? null : parent.TemplatePath) ?? templatePath;
            TextEncoder = textEncoder;
            _value = value;
            _unencodedWriter = GetUnencodedWriter(writer);
            _encodedWriter = GetEncodedWriter(_unencodedWriter, textEncoder);
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
            get
            {
                if (OutputMode == OutputMode.Encoded)
                {
                    return _encodedWriter;
                }
                else
                {
                    return _unencodedWriter;
                }
            }
        }

        public OutputMode OutputMode
        {
            get;
            set; 
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
            var member = target.GetType()
                .GetMember(variableName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (member.Length > 0)
            {
                if (member[0] is PropertyInfo)
                {
                    returnValue = ((PropertyInfo) member[0]).GetValue(target, null);
                }
                else if (member[0] is FieldInfo)
                {
                    returnValue = ((FieldInfo) member[0]).GetValue(target);
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
            return new BindingContext(value, _encodedWriter, this, TemplatePath, TextEncoder);
        }

        private static TextWriter GetEncodedWriter(TextWriter writer, ITextEncoder encoder)
        {
            if (writer is EncodedTextWriter)
            {
                return writer;
            }

            return new EncodedTextWriter(writer, encoder);
        }

        private static TextWriter GetUnencodedWriter(TextWriter writer)
        {
            var encodedTextWriter = writer as EncodedTextWriter;

            return encodedTextWriter != null ? encodedTextWriter.UnderlyingWriter : writer;
        }
    }
}

