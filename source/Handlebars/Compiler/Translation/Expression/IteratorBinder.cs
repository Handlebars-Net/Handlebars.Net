using System;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Collections;

namespace Handlebars.Compiler
{
    internal class IteratorBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, HandlebarsConfiguration configuration)
        {
            return new IteratorBinder(configuration).Visit(expr);
        }

        private readonly HandlebarsConfiguration _configuration;

        private IteratorBinder(HandlebarsConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            return Expression.Block(
                node.Expressions.Select(n => Visit(n)));
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            return Expression.Condition(
                Visit(node.Test),
                Visit(node.IfTrue),
                Visit(node.IfFalse));
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            return Expression.MakeUnary(
                node.NodeType,
                Visit(node.Operand),
                node.Type);
        }

        protected override Expression VisitIteratorExpression(IteratorExpression iex)
        {
            var fb = new FunctionBuilder(_configuration);
            return Expression.Call(
                new Action<BindingContext, IEnumerable, Action<TextWriter, object>, Action<TextWriter, object>>(Iterate).Method,
                new Expression[] {
                    HandlebarsExpression.ContextAccessor(),
                    Expression.Convert(iex.Sequence, typeof(IEnumerable)),
                    Expression.Constant(fb.Compile(new [] { iex.Template })),
                    Expression.Constant(fb.Compile(new [] { iex.IfEmpty })) 
                });
        }

        private static void Iterate(
            BindingContext context,
            IEnumerable sequence,
            Action<TextWriter, object> template,
            Action<TextWriter, object> ifEmpty)
        {
            var count = 0;
            foreach(object item in sequence)
            {
                template(context.TextWriter, item);
            }
            if(count == 0)
            {
                ifEmpty(context.TextWriter, context.Value);
            }
        }
    }
}

