using System;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class DeferredSectionExpression : HandlebarsExpression
    {
        public DeferredSectionExpression(
            PathExpression path,
            BlockExpression body,
            BlockExpression inversion)
        {
            Path = path;
            Body = body;
            Inversion = inversion;
        }

        public BlockExpression Body { get; }

        public BlockExpression Inversion { get; }

        public PathExpression Path { get; }

        public override Type Type => typeof(void);

        public override ExpressionType NodeType => (ExpressionType)HandlebarsExpressionType.DeferredSection;
    }
}

