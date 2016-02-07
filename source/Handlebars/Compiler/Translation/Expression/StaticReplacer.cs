using System;
using System.Linq;
using System.Linq.Expressions;
using System.IO;

namespace HandlebarsDotNet.Compiler
{
    internal class StaticReplacer : HandlebarsExpressionVisitor
    {
        public static Expression Replace(Expression expr, CompilationContext context)
        {
            return new StaticReplacer(context).Visit(expr);
        }

        private StaticReplacer(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            return Expression.Block(
                node.Variables,
                node.Expressions.Select(expr => Visit(expr)));
        }

        protected override Expression VisitStaticExpression(StaticExpression stex)
        {
            var writeMethod = typeof(HandlebarsExtensions).GetMethod("WriteSafeString", new [] { typeof(TextWriter), typeof(string) });
            return Expression.Call(
                writeMethod,
                Expression.Property(
                    CompilationContext.BindingContext,
                    "TextWriter"),
                Expression.Constant(stex.Value));
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            return Expression.Condition(
                node.Test,
                Visit(node.IfTrue),
                Visit(node.IfFalse));
        }
    }
}

