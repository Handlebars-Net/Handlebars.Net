using System;
using System.Collections.Generic;
using System.IO;
using HandlebarsDotNet.Compiler.Resolvers;
using HandlebarsDotNet.Features;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.ObjectDescriptors;

namespace HandlebarsDotNet
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICompiledHandlebarsConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        IExpressionNameResolver ExpressionNameResolver { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ITextEncoder TextEncoder { get; }
        
        /// <summary>
        /// 
        /// </summary>
        IFormatProvider FormatProvider { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ViewEngineFileSystem FileSystem { get; }
        
        /// <summary>
        /// 
        /// </summary>
        string UnresolvedBindingFormatter { get; }
        
        /// <summary>
        /// 
        /// </summary>
        bool ThrowOnUnresolvedBindingExpression { get; }
        
        /// <summary>
        /// 
        /// </summary>
        IPartialTemplateResolver PartialTemplateResolver { get; }
        
        /// <summary>
        /// 
        /// </summary>
        IMissingPartialTemplateHandler MissingPartialTemplateHandler { get; }
        
        /// <summary>
        /// 
        /// </summary>
        IDictionary<string, HandlebarsHelper> Helpers { get; }
        
        /// <summary>
        /// 
        /// </summary>
        IDictionary<string, HandlebarsReturnHelper> ReturnHelpers { get; }
        
        /// <summary>
        /// 
        /// </summary>
        IDictionary<string, HandlebarsBlockHelper> BlockHelpers { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ICollection<IHelperResolver> HelperResolvers { get; }
        
        /// <summary>
        /// 
        /// </summary>
        IDictionary<string, Action<TextWriter, object>> RegisteredTemplates { get; }
        
        /// <inheritdoc cref="Compatibility"/>
        Compatibility Compatibility { get; }
        
        /// <inheritdoc cref="IObjectDescriptorProvider"/>
        IObjectDescriptorProvider ObjectDescriptorProvider { get; }
        
        /// <inheritdoc cref="IExpressionMiddleware"/>
        ICollection<IExpressionMiddleware> ExpressionMiddleware { get; }
        
        /// <inheritdoc cref="IMemberAliasProvider"/>
        ICollection<IMemberAliasProvider> AliasProviders { get; }
        
        /// <inheritdoc cref="IExpressionCompiler"/>
        IExpressionCompiler ExpressionCompiler { get; set; }
        
        /// <inheritdoc cref="CompileTimeConfiguration.UseAggressiveCaching"/>
        bool UseAggressiveCaching { get; set; }
        
        /// <summary>
        /// List of associated <see cref="IFeature"/>s
        /// </summary>
        IReadOnlyList<IFeature> Features { get; }
        
        /// <inheritdoc cref="IReadOnlyPathInfoStore"/>
        IReadOnlyPathInfoStore PathInfoStore { get; }
    }
}