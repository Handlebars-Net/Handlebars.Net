using System;
using System.Linq.Expressions;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Compiler
{
    internal class PathExpression : HandlebarsExpression
    {
        public enum ResolutionContext
        {
            None,
            Parameter
        }
        
        public PathExpression(string path, IReaderContext? readerContext = null)
        {
            Path = path;
            ReaderContext = readerContext;
        }

        public new string Path { get; }

        /// <summary>
        /// Location of the path in the template source, used for error reporting.
        /// </summary>
        public IReaderContext? ReaderContext { get; }
        
        public ResolutionContext Context { get; set; }
        
        public override ExpressionType NodeType => (ExpressionType)HandlebarsExpressionType.PathExpression;

        public override Type Type => typeof(PathInfo);
    }
}

