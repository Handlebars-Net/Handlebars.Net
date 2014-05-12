using System;
using System.Linq;
using System.Linq.Expressions;
using System.IO;

namespace Handlebars.Compiler
{
    internal class StaticReplacer : HandlebarsExpressionVisitor
    {
        public static Expression Replace(Expression expr, CompilationContext context)
        {
            return new StaticReplacer(context).Visit(expr);
        }

        readonly private CompilationContext _context;

        private StaticReplacer(CompilationContext context)
        {
            _context = context;
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            return Expression.Block(
                node.Variables,
                node.Expressions.Select(expr => Visit(expr)));
        }

        protected override Expression VisitStaticExpression(StaticExpression stex)
        {
            var writeMethod = typeof(TextWriter).GetMethod("Write", new [] { typeof(string) });
            return Expression.Call(
                Expression.Property(
                    _context.BindingContext,
                    "TextWriter"),
                writeMethod,
                new[] { Expression.Constant(stex.Value) });
        }
    }
}

