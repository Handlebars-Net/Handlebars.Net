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

        internal override object ReturnInvoke(BindingContext bindingContext, object context, object[] arguments)
        {
            var nameArgument = arguments.Last();
            if (arguments.Length > 1)
            {
                throw new HandlebarsRuntimeException($"Template references a helper that cannot be resolved. Helper '{nameArgument}'");
            }
            
            var name = bindingContext.Configuration.PathInfoStore.GetOrAdd(nameArgument as string ?? nameArgument.ToString());
            return name.GetUndefinedBindingResult(bindingContext.Configuration);
        }

        public override object Invoke(object context, params object[] arguments) => throw new NotSupportedException();
    }
}