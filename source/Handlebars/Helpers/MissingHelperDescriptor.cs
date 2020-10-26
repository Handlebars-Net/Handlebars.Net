using System;
using System.Linq;
using HandlebarsDotNet.Compiler;

namespace HandlebarsDotNet.Helpers
{
    internal sealed class MissingHelperDescriptor : ReturnHelperDescriptor
    {
        public MissingHelperDescriptor() : base("helperMissing")
        {
        }

        internal override object ReturnInvoke(BindingContext bindingContext, object context, in Arguments arguments)
        {
            var nameArgument = arguments[arguments.Length - 1];
            if (arguments.Length > 1)
            {
                throw new HandlebarsRuntimeException($"Template references a helper that cannot be resolved. Helper '{nameArgument}'");
            }
            
            var name = PathInfoStore.Shared.GetOrAdd(nameArgument as string ?? nameArgument.ToString());
            return UndefinedBindingResult.Create(name);
        }

        public override object Invoke(object context, in Arguments arguments) => throw new NotSupportedException();
    }
}