using System;
using System.Collections.Generic;
using System.IO;
using Handlebars.Compiler.Resolvers;

namespace Handlebars
{
    public class HandlebarsConfiguration
    {
        public IDictionary<string, HandlebarsHelper> Helpers { get; private set; }

        public IDictionary<string, HandlebarsBlockHelper> BlockHelpers { get; private set; }

        public IDictionary<string, Action<TextWriter, object>> RegisteredTemplates { get; private set; }

        public IExpressionNameResolver ExpressionNameResolver { get; set; }

        public HandlebarsConfiguration()
        {
            this.Helpers = new Dictionary<string, HandlebarsHelper>(StringComparer.InvariantCultureIgnoreCase);
            this.BlockHelpers = new Dictionary<string, HandlebarsBlockHelper>(StringComparer.InvariantCultureIgnoreCase);
            this.RegisteredTemplates = new Dictionary<string, Action<TextWriter, object>>(StringComparer.InvariantCultureIgnoreCase);
        }
    }
}

