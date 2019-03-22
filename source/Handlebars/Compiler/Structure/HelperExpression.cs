using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace HandlebarsDotNet.Compiler
{
    internal class HelperExpression : HandlebarsExpression
    {
        private readonly IEnumerable<Expression> _arguments;
        private readonly string _helperName;
        private readonly bool _isRaw;

        public HelperExpression(string helperName, IEnumerable<Expression> arguments, bool isRaw = false)
            : this(helperName, isRaw)
        {
            _arguments = arguments;
        }

        public HelperExpression(string helperName, bool isRaw = false)
        {
            _helperName = helperName;
            _isRaw = isRaw;
            _arguments = Enumerable.Empty<Expression>();
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HandlebarsExpressionType.HelperExpression; }
        }

        public override Type Type
        {
            get { return typeof(void); }
        }

        public string HelperName
        {
            get { return _helperName; }
        }

        public bool IsRaw
        {
            get { return _isRaw; }
        }

        public IEnumerable<Expression> Arguments
        {
            get { return _arguments; }
        }
    }
}

