using System.Linq;
using HandlebarsDotNet.Features;

namespace HandlebarsDotNet.Extension.CompileFast
{
    internal class FastCompilerFeatureFactory : IFeatureFactory
    {
        public IFeature CreateFeature()
        {
            return new FastCompilerFeature();
        }
    }
    
    internal class FastCompilerFeature : IFeature
    {
        public void OnCompiling(HandlebarsConfiguration configuration)
        {
            var templateFeature = ((InternalHandlebarsConfiguration) configuration).Features.OfType<ClosureFeature>().SingleOrDefault();
            configuration.CompileTimeConfiguration.ExpressionCompiler = new FastExpressionCompiler(configuration, templateFeature);
        }

        public void CompilationCompleted()
        {
        }
    }
}