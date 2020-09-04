using System;
using System.Collections.Generic;
using System.IO;
using HandlebarsDotNet.Compiler.Resolvers;
using HandlebarsDotNet.Features;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.ObjectDescriptors;

namespace HandlebarsDotNet
{
    
    public interface ICompiledHandlebarsConfiguration
    {
        
        IExpressionNameResolver ExpressionNameResolver { get; }
        
        
        ITextEncoder TextEncoder { get; }
        
        
        IFormatProvider FormatProvider { get; }
        
        
        ViewEngineFileSystem FileSystem { get; }
        
        
        string UnresolvedBindingFormatter { get; }
        
        
        bool ThrowOnUnresolvedBindingExpression { get; }
        
        
        IPartialTemplateResolver PartialTemplateResolver { get; }
        
        
        IMissingPartialTemplateHandler MissingPartialTemplateHandler { get; }
        
        
        IDictionary<string, HandlebarsHelper> Helpers { get; }
        
        
        IDictionary<string, HandlebarsReturnHelper> ReturnHelpers { get; }
        
        
        IDictionary<string, HandlebarsBlockHelper> BlockHelpers { get; }
        
        
        ICollection<IHelperResolver> HelperResolvers { get; }
        
        
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