using HandlebarsDotNet.Collections;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    public sealed class LateBindBlockHelperDescriptor : IHelperDescriptor<BlockHelperOptions>
    {
        public LateBindBlockHelperDescriptor(string name) => Name = name;

        public PathInfo Name { get; }

        public object Invoke(in BlockHelperOptions options, in Context context, in Arguments arguments)
        {
            return this.ReturnInvoke(options, context, arguments);
        }

        public void Invoke(in EncodedTextWriter output, in BlockHelperOptions options, in Context context, in Arguments arguments)
        {
            // TODO: add cache
            var configuration = options.Frame.Configuration;
            var helperResolvers = (ObservableList<IHelperResolver>) configuration.HelperResolvers;
            if(helperResolvers.Count != 0)
            {
                for (var index = 0; index < helperResolvers.Count; index++)
                {
                    if (!helperResolvers[index].TryResolveBlockHelper(Name, out var descriptor)) continue;

                    descriptor.Invoke(output, options, context, arguments);
                    return;
                }
            }

            configuration.BlockHelpers["blockHelperMissing"].Value
                .Invoke(output, options, context, arguments);
        }
    }
}