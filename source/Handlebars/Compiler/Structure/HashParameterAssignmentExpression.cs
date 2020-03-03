using System;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class HashParameterAssignmentExpression : HandlebarsExpression
    {
        public HashParameterAssignmentExpression(string name)
        {
            Name = name;
        }

        public string Name { get; }
        
        public override ExpressionType NodeType => (ExpressionType)HandlebarsExpressionType.HashParameterAssignmentExpression;

        public override Type Type => typeof(object);
    }
}

