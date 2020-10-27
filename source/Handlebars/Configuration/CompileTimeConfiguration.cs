using System.Collections.Generic;
using HandlebarsDotNet.Features;
using HandlebarsDotNet.ObjectDescriptors;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Contains compile-time affective configuration. Changing values after template compilation would take no affect.
    /// </summary>
    public class CompileTimeConfiguration
    {
        /// <inheritdoc cref="ObjectDescriptor"/>
        public IList<IObjectDescriptorProvider> ObjectDescriptorProviders { get; } = new List<IObjectDescriptorProvider>();
        
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