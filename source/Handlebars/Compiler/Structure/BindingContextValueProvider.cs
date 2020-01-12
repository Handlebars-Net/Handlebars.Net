using System.Reflection;

namespace HandlebarsDotNet.Compiler
{
    internal class BindingContextValueProvider : IValueProvider
    {
        private readonly BindingContext _context;

        public BindingContextValueProvider(BindingContext context)
        {
            _context = context;
        }

        public bool ProvidesNonContextVariables { get; } = false;

        public bool TryGetValue(string memberName, out object value)
        {
            value = GetContextVariable(memberName, _context) ?? GetContextVariable(memberName, _context.Value);
            return value != null;
        }
        
        private object GetContextVariable(string variableName, object target)
        {
            variableName = variableName.TrimStart('@');
            var member = target.GetType().GetMember(variableName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (member.Length <= 0) return _context.ParentContext?.GetContextVariable(variableName);
            
            switch (member[0])
            {
                case PropertyInfo propertyInfo:
                    return propertyInfo.GetValue(target, null);
                case FieldInfo fieldInfo:
                    return fieldInfo.GetValue(target);
                default:
                    throw new HandlebarsRuntimeException("Context variable references a member that is not a field or property");
            }
        }
    }
}