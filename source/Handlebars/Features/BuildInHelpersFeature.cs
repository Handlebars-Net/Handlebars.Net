using System.IO;
using System.Linq;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Features
{
    internal class BuildInHelpersFeatureFactory : IFeatureFactory
    {
        public IFeature CreateFeature()
        {
            return new BuildInHelpersFeature();
        }
    }

    [FeatureOrder(int.MinValue)]
    internal class BuildInHelpersFeature : IFeature
    {
        private ICompiledHandlebarsConfiguration _configuration;

        private static readonly ConfigureBlockParams WithBlockParamsConfiguration = (parameters, binder, deps) =>
        {
            binder(parameters.ElementAtOrDefault(0), ctx => ctx, deps[0]);
        };

        public void OnCompiling(ICompiledHandlebarsConfiguration configuration)
        {
            _configuration = configuration;

            configuration.BlockHelpers["with"] = With;
            configuration.BlockHelpers["*inline"] = Inline;

            configuration.ReturnHelpers["lookup"] = Lookup;
        }

        public void CompilationCompleted()
        {
            // noting to do here
        }

        private static void With(TextWriter output, HelperOptions options, dynamic context, params object[] arguments)
        {
            if (arguments.Length != 1)
            {
                throw new HandlebarsException("{{with}} helper must have exactly one argument");
            }

            options.BlockParams(WithBlockParamsConfiguration, arguments[0]);
            
            if (HandlebarsUtils.IsTruthyOrNonEmpty(arguments[0]))
            {
                options.Template(output, arguments[0]);
            }
            else
            {
                options.Inverse(output, context);
            }
        }

        private object Lookup(dynamic context, params object[] arguments)
        {
            if (arguments.Length != 2)
            {
                throw new HandlebarsException("{{lookup}} helper must have exactly two argument");
            }
            
            var memberName = arguments[1].ToString();
            var segment = new ChainSegment(memberName);
            return !PathResolver.TryAccessMember(arguments[0], ref segment, _configuration, out var value) 
                ? new UndefinedBindingResult(memberName, _configuration)
                : value;
        }

        private static void Inline(TextWriter output, HelperOptions options, dynamic context, params object[] arguments)
        {
            if (arguments.Length != 1)
            {
                throw new HandlebarsException("{{*inline}} helper must have exactly one argument");
            }

            //This helper needs the "context" var to be the complete BindingContext as opposed to just the
            //data { firstName: "todd" }. The full BindingContext is needed for registering the partial templates.
            //This magic happens in BlockHelperFunctionBinder.VisitBlockHelperExpression

            if (!(context is BindingContext))
            {
                throw new HandlebarsException("{{*inline}} helper must receiving the full BindingContext");
            }

            var key = arguments[0] as string;
            
            //Inline partials cannot use the Handlebars.RegisterTemplate method
            //because it is static and therefore app-wide. To prevent collisions
            //this helper will add the compiled partial to a dicionary
            //that is passed around in the context without fear of collisions.
            context.InlinePartialTemplates.Add(key, options.Template);
        }
    }
}