using System;
using System.Linq.Expressions;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Compiler
{
    internal class PathExpression : HandlebarsExpression
    {
        public enum ResolutionContext
        {
            None,
            Parameter
        }
        
        public PathExpression(string path)
        {
            Path = path;
        }

        public new string Path { get; }
        
        public ResolutionContext Context { get; set; }
        
        public override ExpressionType NodeType => (ExpressionType)HandlebarsExpressionType.PathExpression;

        public override Type Type => typeof(PathInfo);
    }
}

