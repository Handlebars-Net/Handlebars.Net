using System;
using System.Collections.Generic;
using System.IO;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler.Resolvers;
using HandlebarsDotNet.Decorators;
using HandlebarsDotNet.Features;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.IO;
using HandlebarsDotNet.ObjectDescriptors;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet
{
    public interface IHandlebarsTemplateRegistrations
    {
        IIndexed<string, HandlebarsTemplate<TextWriter, object, object>> RegisteredTemplates { get; }
        ViewEngineFileSystem FileSystem { get; }
    }
    
    public interface ICompiledHandlebarsConfiguration : IHandlebarsTemplateRegistrations
    {
        HandlebarsConfiguration UnderlingConfiguration { get; }
        
        IExpressionNameResolver ExpressionNameResolver { get; }
        
        ITextEncoder TextEncoder { get; }
        
        IFormatProvider FormatProvider { get; }
        
        ObservableList<IFormatterProvider> FormatterProviders { get; }
        
        bool ThrowOnUnresolvedBindingExpression { get; }
        
        IPartialTemplateResolver PartialTemplateResolver { get; }
        
        IMissingPartialTemplateHandler MissingPartialTemplateHandler { get; }
        
        IIndexed<PathInfoLight, Ref<IHelperDescriptor<HelperOptions>>> Helpers { get; }
        
        IIndexed<PathInfoLight, Ref<IHelperDescriptor<BlockHelperOptions>>> BlockHelpers { get; }
        
        IIndexed<PathInfoLight, Ref<IDecoratorDescriptor<DecoratorOptions>>> Decorators { get; }
        
        IIndexed<PathInfoLight, Ref<IDecoratorDescriptor<BlockDecoratorOptions>>> BlockDecorators { get; }
        
        IAppendOnlyList<IHelperResolver> HelperResolvers { get; }
        
        /// <inheritdoc cref="Compatibility"/>
        Compatibility Compatibility { get; }
        
        /// <inheritdoc cref="IObjectDescriptorProvider"/>
        ObservableList<IObjectDescriptorProvider> ObjectDescriptorProviders { get; }
        
        /// <inheritdoc cref="IExpressionMiddleware"/>
        IAppendOnlyList<IExpressionMiddleware> ExpressionMiddlewares { get; }
        
        /// <inheritdoc cref="IMemberAliasProvider"/>
        IAppendOnlyList<IMemberAliasProvider> AliasProviders { get; }
        
        /// <inheritdoc cref="IExpressionCompiler"/>
        IExpressionCompiler ExpressionCompiler { get; set; }

        /// <summary>
        /// List of associated <see cref="IFeature"/>s
        /// </summary>
        IReadOnlyList<IFeature> Features { get; }
        
        bool NoEscape { get; }
    }
}