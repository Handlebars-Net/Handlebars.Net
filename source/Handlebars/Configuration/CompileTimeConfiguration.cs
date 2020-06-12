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
        
        /// <summary>
        /// 
        /// </summary>
        public IList<IExpressionMiddleware> ExpressionMiddleware { get; internal set; } = new List<IExpressionMiddleware>();
        
        /// <inheritdoc cref="IFeature"/>
        public IList<IFeatureFactory> Features { get; internal set; } = new List<IFeatureFactory>
        {
            new BuildInHelpersFeatureFactory(),
            new ClosureFeatureFactory(),
            new DefaultCompilerFeatureFactory(),
            new MissingHelperFeatureFactory()
        };
        
        /// <inheritdoc cref="IMemberAliasProvider"/>
        public IList<IMemberAliasProvider> AliasProviders { get; internal set; } = new List<IMemberAliasProvider>();
        
        /// <summary>
        /// Defines whether Handlebars uses aggressive caching to achieve better performance. <see langword="true"/> by default. 
        /// </summary>
        public bool UseAggressiveCaching { get; set; } = true;
        
        /// <summary>
        /// The compiler used to compile <see cref="System.Linq.Expressions.Expression"/> 
        /// </summary>
        public IExpressionCompiler ExpressionCompiler { get; set; }
    }
}