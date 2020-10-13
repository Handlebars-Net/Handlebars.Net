using System;
using System.Linq;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers
{
    internal sealed class LateBindHelperDescriptor : ReturnHelperDescriptor
    {
        private readonly ICompiledHandlebarsConfiguration _configuration;
        private readonly StrongBox<HelperDescriptorBase> _helperMissing;

        public LateBindHelperDescriptor(PathInfo name, ICompiledHandlebarsConfiguration configuration) : base(name)
        {
            _configuration = configuration;
            var pathInfoStore = _configuration.PathInfoStore;
            _helperMissing = _configuration.Helpers[pathInfoStore.GetOrAdd("helperMissing")];
        }
        
        public LateBindHelperDescriptor(string name, ICompiledHandlebarsConfiguration configuration) : base(configuration.PathInfoStore.GetOrAdd(name))
        {
            _configuration = configuration;
            var pathInfoStore = _configuration.PathInfoStore;
            _helperMissing = _configuration.Helpers[pathInfoStore.GetOrAdd("helperMissing")];
        }

        internal override object ReturnInvoke(BindingContext bindingContext, object context, object[] arguments)
        {
            var helperResolvers = (ObservableList<IHelperResolver>) _configuration.HelperResolvers;
            if(helperResolvers.Count != 0)
            {
                var targetType = arguments.FirstOrDefault()?.GetType();
                for (var index = 0; index < helperResolvers.Count; index++)
                {
                    var resolver = helperResolvers[index];
                    if (!resolver.TryResolveHelper(Name, targetType, out var helper)) continue;

                    return helper.ReturnInvoke(bindingContext, context, arguments);
                }
            }

            var value = PathResolver.ResolvePath(bindingContext, Name);
            if (!(value is UndefinedBindingResult)) return value;

            var nameIndex = arguments.Length;
            Array.Resize(ref arguments, nameIndex + 1);
            arguments[nameIndex] = Name.TrimmedPath;
            return _helperMissing.Value.ReturnInvoke(bindingContext, context, arguments);
        }

        public override object Invoke(object context, params object[] arguments)
        {
            throw new NotImplementedException();
        }
    }
}