using System;
using Handlebars.Compiler;
using System.Linq.Expressions;

namespace Handlebars.Compiler
{
    internal class PartialExpression : HandlebarsExpression
    {
        private readonly string _partialName;
        private readonly string _objectPassedIn;

        public PartialExpression(string partialName, string objectPassedIn)
        {
            _partialName = partialName;
            _objectPassedIn = objectPassedIn;
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HandlebarsExpressionType.PartialExpression; }
        }

        public override Type Type
        {
            get { return typeof(void); }
        }

        public string PartialName
        {
            get { return _partialName; }
        }

        public string ObjectPassedIn
        {
            get { return _objectPassedIn; }
        }
    }
}

