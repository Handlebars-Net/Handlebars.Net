using HandlebarsDotNet.Compiler.Resolvers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using HandlebarsDotNet.Helpers;

namespace HandlebarsDotNet
{
    
    public class HandlebarsConfiguration
    {
        
        public IDictionary<string, HandlebarsHelper> Helpers { get; protected set; }
        
        
        public IDictionary<string, HandlebarsReturnHelper> ReturnHelpers { get; protected set; }

        
        public IDictionary<string, HandlebarsBlockHelper> BlockHelpers { get; protected set; }

        
        public IDictionary<string, Action<TextWriter, object>> RegisteredTemplates { get; protected set; }
        
        /// <inheritdoc cref="HandlebarsDotNet.Helpers.IHelperResolver"/>
        public ICollection<IHelperResolver> HelperResolvers { get; protected set; }

        
        public virtual IExpressionNameResolver ExpressionNameResolver { get; set; }

        
        public virtual ITextEncoder TextEncoder { get; set; } = new HtmlEncoder();
        
        /// <inheritdoc cref="IFormatProvider"/>
        public virtual IFormatProvider FormatProvider { get; set; } = CultureInfo.CurrentCulture;

        
        public virtual ViewEngineFileSystem FileSystem { get; set; }

        
	    public virtual string UnresolvedBindingFormatter { get; set; }

        
        public virtual bool ThrowOnUnresolvedBindingExpression { get; set; }

        /// <summary>
        /// The resolver used for unregistered partials. Defaults
        /// to the <see cref="FileSystemPartialTemplateResolver"/>.
        /// </summary>
        public virtual IPartialTemplateResolver PartialTemplateResolver { get; set; } = new FileSystemPartialTemplateResolver();

        /// <summary>
        /// The handler called when a partial template cannot be found.
        /// </summary>
        public virtual IMissingPartialTemplateHandler MissingPartialTemplateHandler { get; set; }

        /// <inheritdoc cref="HandlebarsDotNet.Compatibility"/>
        public virtual Compatibility Compatibility { get; set; } = new Compatibility();

        /// <inheritdoc cref="HandlebarsDotNet.CompileTimeConfiguration"/>
        public virtual CompileTimeConfiguration CompileTimeConfiguration { get; } = new CompileTimeConfiguration();

        
        public HandlebarsConfiguration()
        {
            Helpers = new ConcurrentDictionary<string, HandlebarsHelper>(StringComparer.OrdinalIgnoreCase);
            ReturnHelpers = new ConcurrentDictionary<string, HandlebarsReturnHelper>(StringComparer.OrdinalIgnoreCase);
            BlockHelpers = new ConcurrentDictionary<string, HandlebarsBlockHelper>(StringComparer.OrdinalIgnoreCase);
            RegisteredTemplates = new ConcurrentDictionary<string, Action<TextWriter, object>>(StringComparer.OrdinalIgnoreCase);
            HelperResolvers = new List<IHelperResolver>();
        }
    }
}

