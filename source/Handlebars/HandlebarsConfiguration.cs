using System;
using System.Collections.Generic;

namespace Handlebars
{
    public class HandlebarsConfiguration
    {
        public IDictionary<string, HandlebarsHelper> Helpers { get; private set; }
        public IDictionary<string, HandlebarsBlockHelper> BlockHelpers { get; private set; }


        public HandlebarsConfiguration ()
        {
            this.Helpers = new Dictionary<string, HandlebarsHelper>(StringComparer.InvariantCultureIgnoreCase);
            this.BlockHelpers = new Dictionary<string, HandlebarsBlockHelper>(StringComparer.InvariantCultureIgnoreCase);
        }
    }
}

