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
        public void OnCompiling(ICompiledHandlebarsConfiguration configuration)
        {
            var templateFeature = configuration.Features.OfType<ClosureFeature>().SingleOrDefault();
            configuration.ExpressionCompiler = new FastExpressionCompiler(configuration, templateFeature);
        }

        public void CompilationCompleted()
        {
        }
    }
}