using System.Collections.Generic;
using HandlebarsDotNet.Features;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Contains compile-time affective configuration. Changing values after template compilation would take no affect.
    /// </summary>
    public class CompileTimeConfiguration
    {
        public IList<IExpressionMiddleware> ExpressionMiddleware { get; } = new List<IExpressionMiddleware>();
        
        /// <inheritdoc cref="IFeature"/>
        public IList<IFeatureFactory> Features { get; } = new List<IFeatureFactory>
        {
            new BuildInHelpersFeatureFactory(),
            new DefaultCompilerFeatureFactory(),
            new MissingHelperFeatureFactory()
        };

        /// <summary>
        /// The compiler used to compile <see cref="System.Linq.Expressions.Expression"/> 
        /// </summary>
        public IExpressionCompiler ExpressionCompiler { get; set; }
    }
}