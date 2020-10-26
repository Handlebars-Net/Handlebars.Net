using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.Compiler.Resolvers;
using HandlebarsDotNet.Features;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.BlockHelpers;
using HandlebarsDotNet.ObjectDescriptors;

namespace HandlebarsDotNet
{
    public interface IHandlebarsTemplateRegistrations
    {
        IDictionary<string, HandlebarsTemplate<TextWriter, object, object>> RegisteredTemplates { get; }
        ViewEngineFileSystem FileSystem { get; }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public interface ICompiledHandlebarsConfiguration : IHandlebarsTemplateRegistrations
    {
        /// <summary>
        /// 
        /// </summary>
        HandlebarsConfiguration UnderlingConfiguration { get; }
        
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
        
        [Obsolete("Use `UnresolvedBindingFormat` instead")]
        string UnresolvedBindingFormatter { get; }
        
        Func<UndefinedBindingResult, string> UnresolvedBindingFormat { get; }
        
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
        IDictionary<PathInfoLight, StrongBox<HelperDescriptorBase>> Helpers { get; }
        
        /// <summary>
        /// 
        /// </summary>
        IDictionary<PathInfoLight, StrongBox<BlockHelperDescriptorBase>> BlockHelpers { get; }
        
        /// <summary>
        /// 
        /// </summary>
        IList<IHelperResolver> HelperResolvers { get; }
        
        /// <inheritdoc cref="Compatibility"/>
        Compatibility Compatibility { get; }
        
        /// <inheritdoc cref="IObjectDescriptorProvider"/>
        IObjectDescriptorProvider ObjectDescriptorProvider { get; }
        
        /// <inheritdoc cref="IExpressionMiddleware"/>
        IList<IExpressionMiddleware> ExpressionMiddlewares { get; }
        
        /// <inheritdoc cref="IMemberAliasProvider"/>
        IList<IMemberAliasProvider> AliasProviders { get; }
        
        /// <inheritdoc cref="IExpressionCompiler"/>
        IExpressionCompiler ExpressionCompiler { get; set; }

        /// <summary>
        /// List of associated <see cref="IFeature"/>s
        /// </summary>
        IReadOnlyList<IFeature> Features { get; }
        
        /// <inheritdoc cref="IPathInfoStore"/>
        IPathInfoStore PathInfoStore { get; }
        
        bool NoEscape { get; }
    }
}