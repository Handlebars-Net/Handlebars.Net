using System;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class PathExpression : HandlebarsExpression
    {
        public PathExpression(string path)
        {
            Path = path;
            PathInfo = PathResolver.GetPathInfo(path);
        }

        public string Path { get; }
        
        public PathInfo PathInfo { get; }

        public override ExpressionType NodeType => (ExpressionType)HandlebarsExpressionType.PathExpression;

        public override Type Type => typeof(object);
    }
}

