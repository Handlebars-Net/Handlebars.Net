using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Decorators
{
    internal sealed class InlineBlockDecoratorDescriptor : IDecoratorDescriptor<BlockDecoratorOptions>
    {
        public TemplateDelegate Invoke(in TemplateDelegate function, in BlockDecoratorOptions options, in Context context, in Arguments arguments)
        {
            if (arguments.Length != 1)
            {
                throw new HandlebarsRuntimeException("{{*inline}} helper must have exactly one argument");
            }

            var bindingContext = options.Frame;
            
            if(arguments[0] is not string key) throw new HandlebarsRuntimeException("Inline argument is not valid");
            
            //Inline partials cannot use the Handlebars.RegisterTemplate method
            //because it is static and therefore app-wide. To prevent collisions
            //this helper will add the compiled partial to a dicionary
            //that is passed around in the context without fear of collisions.
            var template = options.OriginalTemplate;
            bindingContext.InlinePartialTemplates.AddOrReplace(key, (writer, c) =>
            {
                template(writer, c);
            });

            return function;
        }

        public PathInfo Name { get; } = "*inline";
    }
}