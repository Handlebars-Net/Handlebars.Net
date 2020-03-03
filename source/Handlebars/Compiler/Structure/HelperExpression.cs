using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace HandlebarsDotNet.Compiler
{
    internal class HelperExpression : HandlebarsExpression
    {
        public HelperExpression(string helperName, IEnumerable<Expression> arguments, bool isRaw = false)
            : this(helperName, isRaw)
        {
            Arguments = arguments;
        }

        public HelperExpression(string helperName, bool isRaw = false)
        {
            HelperName = helperName;
            IsRaw = isRaw;
            Arguments = Enumerable.Empty<Expression>();
        }

        public override ExpressionType NodeType => (ExpressionType)HandlebarsExpressionType.HelperExpression;

        public override Type Type => typeof(void);

        public string HelperName { get; }

        public bool IsRaw { get; }

        public IEnumerable<Expression> Arguments { get; }
    }
}

