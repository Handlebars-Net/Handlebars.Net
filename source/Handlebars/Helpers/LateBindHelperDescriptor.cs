using HandlebarsDotNet.Collections;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Helpers
{
    public sealed class LateBindHelperDescriptor : IHelperDescriptor<HelperOptions>
    {
        public LateBindHelperDescriptor(string name) => Name = name;

        public PathInfo Name { get; }

        public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
        {
            var bindingContext = options.Frame;
            
            // TODO: add cache
            var configuration = options.Frame.Configuration;
            var helperResolvers = (ObservableList<IHelperResolver>) configuration.HelperResolvers;
            if(helperResolvers.Count != 0)
            {
                var targetType = arguments.Length > 0 ? arguments[0].GetType() : null;
                for (var index = 0; index < helperResolvers.Count; index++)
                {
                    var resolver = helperResolvers[index];
                    if (!resolver.TryResolveHelper(Name, targetType, out var helper)) continue;

                    return helper.Invoke(options, context, arguments);
                }
            }

            var value = PathResolver.ResolvePath(bindingContext, Name);
            if (!(value is UndefinedBindingResult)) return value;
            
            return configuration.Helpers["helperMissing"].Value.Invoke(options, context, arguments);
        }

        public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
        {
            output.Write(Invoke(options, context, arguments));
        }
    }
}