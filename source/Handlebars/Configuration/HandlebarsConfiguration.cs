using HandlebarsDotNet.Compiler.Resolvers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.BlockHelpers;

namespace HandlebarsDotNet
{
    public sealed class HandlebarsConfiguration : IHandlebarsTemplateRegistrations
    {
        public IDictionary<string, HelperDescriptorBase> Helpers { get; }
        
        public IDictionary<string, BlockHelperDescriptorBase> BlockHelpers { get; }
        
        public IDictionary<string, HandlebarsTemplate<TextWriter, object, object>> RegisteredTemplates { get; }
        
        /// <inheritdoc cref="HandlebarsDotNet.Helpers.IHelperResolver"/>
        public IList<IHelperResolver> HelperResolvers { get; }
        
        public IExpressionNameResolver ExpressionNameResolver { get; set; }
        
        public ITextEncoder TextEncoder { get; set; }
        
        /// <inheritdoc cref="IFormatProvider"/>
        public IFormatProvider FormatProvider { get; set; } = CultureInfo.CurrentCulture;
        
        public ViewEngineFileSystem FileSystem { get; set; }
        
        [Obsolete("Use `UnresolvedBindingFormat` instead.")]
	    public string UnresolvedBindingFormatter { get; set; }
        
        public Func<UndefinedBindingResult, string> UnresolvedBindingFormat { get; set; }
        
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
        public IList<IMemberAliasProvider> AliasProviders { get; internal set; } = new List<IMemberAliasProvider>();

        /// <inheritdoc cref="HandlebarsDotNet.Compatibility"/>
        public Compatibility Compatibility { get; } = new Compatibility();

        /// <inheritdoc cref="HandlebarsDotNet.CompileTimeConfiguration"/>
        public CompileTimeConfiguration CompileTimeConfiguration { get; } = new CompileTimeConfiguration();
        
        public HandlebarsConfiguration()
        {
            Helpers = new ObservableDictionary<string, HelperDescriptorBase>(comparer: StringComparer.OrdinalIgnoreCase);
            BlockHelpers = new ObservableDictionary<string, BlockHelperDescriptorBase>(comparer: StringComparer.OrdinalIgnoreCase);
            RegisteredTemplates = new ObservableDictionary<string, HandlebarsTemplate<TextWriter, object, object>>(comparer: StringComparer.OrdinalIgnoreCase);
            HelperResolvers = new ObservableList<IHelperResolver>();
            TextEncoder = new HtmlEncoder(FormatProvider);
        }
    }
}

