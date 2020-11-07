using System.Runtime.CompilerServices;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    internal sealed class LateBindBlockHelperDescriptor : BlockHelperDescriptor
    {
        private readonly  Ref<BlockHelperDescriptorBase> _blockHelperMissing;
        private readonly ObservableList<IHelperResolver> _helperResolvers;

        public LateBindBlockHelperDescriptor(string name, ICompiledHandlebarsConfiguration configuration) : base(configuration.PathInfoStore.GetOrAdd(name))
        {
            _helperResolvers = (ObservableList<IHelperResolver>) configuration.HelperResolvers;
            _blockHelperMissing = configuration.BlockHelpers["blockHelperMissing"];
        }
        
        public override void Invoke(in EncodedTextWriter output, in BlockHelperOptions options, object context, in Arguments arguments)
        {
            // TODO: add cache
            if(_helperResolvers.Count != 0)
            {
                for (var index = 0; index < _helperResolvers.Count; index++)
                {
                    if (!_helperResolvers[index].TryResolveBlockHelper(Name, out var descriptor)) continue;

                    descriptor.Invoke(output, options, context, arguments);
                    return;
                }
            }

            options["name"] = Name.TrimmedPath;
            options["path"] = Name;
            _blockHelperMissing.Value.Invoke(output, options, context, arguments);
        }
    }
}