using HandlebarsDotNet.Compiler.Resolvers;
using System;
using System.Globalization;
using System.IO;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.EqualityComparers;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.ObjectDescriptors;

namespace HandlebarsDotNet
{
    public sealed class HandlebarsConfiguration : IHandlebarsTemplateRegistrations
    {
        public IIndexed<string, IHelperDescriptor<HelperOptions>> Helpers { get; }
        
        public IIndexed<string, IHelperDescriptor<BlockHelperOptions>> BlockHelpers { get; }
        
        public IIndexed<string, HandlebarsTemplate<TextWriter, object, object>> RegisteredTemplates { get; }
        
        /// <inheritdoc cref="HandlebarsDotNet.Helpers.IHelperResolver"/>
        public IAppendOnlyList<IHelperResolver> HelperResolvers { get; }
        
        public IExpressionNameResolver ExpressionNameResolver { get; set; }
        
        public ITextEncoder TextEncoder { get; set; }
        
        /// <inheritdoc cref="IFormatProvider"/>
        public IFormatProvider FormatProvider { get; set; } = CultureInfo.CurrentCulture;
        
        public ViewEngineFileSystem FileSystem { get; set; }
        
	    public Formatter<UndefinedBindingResult> UnresolvedBindingFormatter { get; set; }
        
        public bool ThrowOnUnresolvedBindingExpression { get; set; }
        
        public bool NoEscape { get; set; }

        /// <summary>
        /// The resolver used for unregistered partials. Defaults
        /// to the <see cref="FileSystemPartialTemplateResolver"/>.
        /// </summary>
        public IPartialTemplateResolver PartialTemplateResolver { get; set; } = new FileSystemPartialTemplateResolver();

        /// <summary>
        /// The handler called when a partial template cannot be found.
        /// </summary>
        public IMissingPartialTemplateHandler MissingPartialTemplateHandler { get; set; }
        
        /// <inheritdoc cref="IMemberAliasProvider"/>
        public IAppendOnlyList<IMemberAliasProvider> AliasProviders { get; internal set; } = new ObservableList<IMemberAliasProvider>();

        /// <inheritdoc cref="HandlebarsDotNet.Compatibility"/>
        public Compatibility Compatibility { get; } = new Compatibility();

        /// <inheritdoc cref="HandlebarsDotNet.CompileTimeConfiguration"/>
        public CompileTimeConfiguration CompileTimeConfiguration { get; } = new CompileTimeConfiguration();

        public IAppendOnlyList<IObjectDescriptorProvider> ObjectDescriptorProviders { get; } = new ObservableList<IObjectDescriptorProvider>();

        public HandlebarsConfiguration()
        {
            var stringEqualityComparer = new StringEqualityComparer(StringComparison.OrdinalIgnoreCase);
            Helpers = new ObservableIndex<string, IHelperDescriptor<HelperOptions>, StringEqualityComparer>(stringEqualityComparer);
            BlockHelpers = new ObservableIndex<string, IHelperDescriptor<BlockHelperOptions>, StringEqualityComparer>(stringEqualityComparer);
            RegisteredTemplates = new ObservableIndex<string, HandlebarsTemplate<TextWriter, object, object>, StringEqualityComparer>(stringEqualityComparer);
            
            HelperResolvers = new ObservableList<IHelperResolver>();
            TextEncoder = new HtmlEncoder(FormatProvider);
            UnresolvedBindingFormatter = new Formatter<UndefinedBindingResult>(undef => string.Empty);
        }
    }
}

