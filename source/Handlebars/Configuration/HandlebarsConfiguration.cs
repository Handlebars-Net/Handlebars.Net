using HandlebarsDotNet.Compiler.Resolvers;
using System;
using System.Globalization;
using System.IO;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Decorators;
using HandlebarsDotNet.EqualityComparers;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.IO;
using HandlebarsDotNet.ObjectDescriptors;

namespace HandlebarsDotNet
{
    public sealed class HandlebarsConfiguration : IHandlebarsTemplateRegistrations
    {
        private readonly UndefinedFormatter _undefinedFormatter = new UndefinedFormatter();

        public IIndexed<string, IHelperDescriptor<HelperOptions>> Helpers { get; }
        
        public IIndexed<string, IHelperDescriptor<BlockHelperOptions>> BlockHelpers { get; }
        
        public IIndexed<string, IDecoratorDescriptor<DecoratorOptions>> Decorators { get; }
        
        public IIndexed<string, IDecoratorDescriptor<BlockDecoratorOptions>> BlockDecorators { get; }
        
        public IIndexed<string, HandlebarsTemplate<TextWriter, object, object>> RegisteredTemplates { get; }
        
        /// <inheritdoc cref="HandlebarsDotNet.Helpers.IHelperResolver"/>
        public IAppendOnlyList<IHelperResolver> HelperResolvers { get; }
        
        public IExpressionNameResolver ExpressionNameResolver { get; set; }
        
        public ITextEncoder TextEncoder { get; set; }
        
        /// <inheritdoc cref="IFormatProvider"/>
        public IFormatProvider FormatProvider { get; set; } = CultureInfo.CurrentCulture;
        
        public ViewEngineFileSystem FileSystem { get; set; }

        [Obsolete("Register custom formatters using `Formatters` property")]
        public string UnresolvedBindingFormatter
        {
            get => _undefinedFormatter.FormatString;
            set => _undefinedFormatter.FormatString = value;
        }

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
        
        /// <summary>
        /// Maximum depth to recurse into partial templates when evaluating the template. Defaults to 100.
        /// </summary>
        public short PartialRecursionDepthLimit { get; set; } = 100;
        
        /// <inheritdoc cref="IMemberAliasProvider"/>
        public IAppendOnlyList<IMemberAliasProvider> AliasProviders { get; } = new ObservableList<IMemberAliasProvider>();

        /// <inheritdoc cref="HandlebarsDotNet.Compatibility"/>
        public Compatibility Compatibility { get; } = new Compatibility();

        /// <inheritdoc cref="HandlebarsDotNet.CompileTimeConfiguration"/>
        public CompileTimeConfiguration CompileTimeConfiguration { get; } = new CompileTimeConfiguration();

        public ObservableList<IFormatterProvider> FormatterProviders { get; } = new ObservableList<IFormatterProvider>();

        /// <inheritdoc cref="ObjectDescriptor"/>
        public ObservableList<IObjectDescriptorProvider> ObjectDescriptorProviders { get; } = new ObservableList<IObjectDescriptorProvider>();

        public HandlebarsConfiguration()
        {
            var stringEqualityComparer = new StringEqualityComparer(StringComparison.OrdinalIgnoreCase);
            Helpers = new ObservableIndex<string, IHelperDescriptor<HelperOptions>, StringEqualityComparer>(stringEqualityComparer);
            BlockHelpers = new ObservableIndex<string, IHelperDescriptor<BlockHelperOptions>, StringEqualityComparer>(stringEqualityComparer);
            Decorators = new ObservableIndex<string, IDecoratorDescriptor<DecoratorOptions>, StringEqualityComparer>(stringEqualityComparer);
            BlockDecorators = new ObservableIndex<string, IDecoratorDescriptor<BlockDecoratorOptions>, StringEqualityComparer>(stringEqualityComparer);
            RegisteredTemplates = new ObservableIndex<string, HandlebarsTemplate<TextWriter, object, object>, StringEqualityComparer>(stringEqualityComparer);
            
            HelperResolvers = new ObservableList<IHelperResolver>();
            TextEncoder = new HtmlEncoderLegacy();
            FormatterProviders.Add(_undefinedFormatter);
        }
    }
}

