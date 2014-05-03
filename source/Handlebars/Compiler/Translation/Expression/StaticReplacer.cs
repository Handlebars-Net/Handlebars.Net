using System;
using System.Linq;
using System.Linq.Expressions;
using System.IO;

namespace Handlebars.Compiler
{
    internal class StaticReplacer : HandlebarsExpressionVisitor
    {
        public static Expression Replace(Expression expr)
        {
            return new StaticReplacer().Visit(expr);
        }

        private StaticReplacer()
        {
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            return Expression.Block(
                node.Expressions.Select(expr => Visit(expr)));
        }

        protected override Expression VisitStaticExpression(StaticExpression stex)
        {
            var writeMethod = typeof(TextWriter).GetMethod("Write", new [] { typeof(string) });
            return Expression.Call(
                Expression.Property(
                    HandlebarsExpression.ContextAccessor(),
                    "TextWriter"),
                writeMethod,
                new[] { Expression.Constant(stex.Value) });
        }
    }
}

