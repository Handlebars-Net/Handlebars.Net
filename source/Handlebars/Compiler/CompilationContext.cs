using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Handlebars.Compiler
{
    internal class CompilationContext
    {
        private readonly HandlebarsConfiguration _configuration;
        private readonly ParameterExpression _bindingContext;

        public CompilationContext(HandlebarsConfiguration configuration)
        {
            _configuration = configuration;
            _bindingContext = Expression.Variable(typeof(BindingContext), "bindingContext");
        }

        public HandlebarsConfiguration Configuration
        {
            get { return _configuration; }
        }

        public ParameterExpression BindingContext
        {
            get { return _bindingContext; }
        }
    }
}
