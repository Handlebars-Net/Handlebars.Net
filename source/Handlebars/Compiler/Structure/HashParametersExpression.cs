using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class HashParametersExpression : HandlebarsExpression
    {
        public Dictionary<string, Expression> Parameters { get; }

        public HashParametersExpression(Dictionary<string, Expression> parameters)
        {
            Parameters = parameters;
        }

        public override ExpressionType NodeType => (ExpressionType)HandlebarsExpressionType.HashParametersExpression;

        public override Type Type => typeof(HashParameterDictionary);
    }
}

