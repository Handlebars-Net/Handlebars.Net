using System.Runtime.CompilerServices;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet.Helpers
{
    internal sealed class LateBindHelperDescriptor : ReturnHelperDescriptor
    {
        private readonly Ref<HelperDescriptorBase> _helperMissing;
        private readonly ObservableList<IHelperResolver> _helperResolvers;

        public LateBindHelperDescriptor(string name, ICompiledHandlebarsConfiguration configuration) : base(name)
        {
            _helperMissing = configuration.Helpers["helperMissing"];
            _helperResolvers = (ObservableList<IHelperResolver>) configuration.HelperResolvers;
        }

        protected override object Invoke(in HelperOptions options, object context, in Arguments arguments)
        {
            var bindingContext = options.Frame;
            
            // TODO: add cache
            if(_helperResolvers.Count != 0)
            {
                var targetType = arguments.Length > 0 ? arguments[0].GetType() : null;
                for (var index = 0; index < _helperResolvers.Count; index++)
                {
                    var resolver = _helperResolvers[index];
                    if (!resolver.TryResolveHelper(Name, targetType, out var helper)) continue;

                    return helper.ReturnInvoke(bindingContext, context, arguments);
                }
            }

            var value = PathResolver.ResolvePath(bindingContext, Name);
            if (!(value is UndefinedBindingResult)) return value;
            
            var newArguments = arguments.Add(Name.TrimmedPath);
            return _helperMissing.Value.ReturnInvoke(bindingContext, context, newArguments);
        }
    }
}