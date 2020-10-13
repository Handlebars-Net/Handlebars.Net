using System.IO;
using HandlebarsDotNet.Compiler;

namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    internal sealed class InlineBlockHelperDescriptor : BlockHelperDescriptor
    {
        public InlineBlockHelperDescriptor() : base("*inline")
        {
        }

        public override void Invoke(TextWriter output, HelperOptions options, object context, params object[] arguments)
        {
            if (arguments.Length != 1)
            {
                throw new HandlebarsException("{{*inline}} helper must have exactly one argument");
            }

            //This helper needs the "context" var to be the complete BindingContext as opposed to just the
            //data { firstName: "todd" }. The full BindingContext is needed for registering the partial templates.
            //This magic happens in BlockHelperFunctionBinder.VisitBlockHelperExpression

            if (!(context is BindingContext bindingContext))
            {
                throw new HandlebarsException("{{*inline}} helper must receiving the full BindingContext");
            }

            if(!(arguments[0] is string key)) throw new HandlebarsRuntimeException("Inline argument is not valid");
            
            //Inline partials cannot use the Handlebars.RegisterTemplate method
            //because it is static and therefore app-wide. To prevent collisions
            //this helper will add the compiled partial to a dicionary
            //that is passed around in the context without fear of collisions.
            var template = options.OriginalTemplate;
            bindingContext.InlinePartialTemplates.Add(key, (writer, o) => template(bindingContext, writer, o));
        }
    }
}