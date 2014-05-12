using System;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Collections;

namespace Handlebars.Compiler
{
    internal class IteratorBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new IteratorBinder(context).Visit(expr);
        }

        private readonly CompilationContext _context;

        private IteratorBinder(CompilationContext context)
        {
            _context = context;
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            return Expression.Block(
                node.Variables,
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
            var fb = new FunctionBuilder(_context.Configuration);
            var iteratorBindingContext = Expression.Variable(typeof(IteratorBindingContext));
            return Expression.Block(
                new ParameterExpression[] {
                    iteratorBindingContext
                },
                Expression.Assign(iteratorBindingContext,
                        Expression.New(
                            typeof(IteratorBindingContext).GetConstructor(new[] { typeof(BindingContext) }),
                            new Expression[] { _context.BindingContext })),
                Expression.Call(
                    new Action<IteratorBindingContext, IEnumerable, Action<TextWriter, object>, Action<TextWriter, object>>(Iterate).Method,
                    new Expression[] {
                        iteratorBindingContext,
                        Expression.Convert(iex.Sequence, typeof(IEnumerable)),
                        fb.Compile(new [] { iex.Template }, iteratorBindingContext),
                        fb.Compile(new [] { iex.IfEmpty }, _context.BindingContext) 
                    }));
        }

        //TODO: make this a little less dumb
        private static void Iterate(
            IteratorBindingContext context,
            IEnumerable sequence,
            Action<TextWriter, object> template,
            Action<TextWriter, object> ifEmpty)
        {
            context.Index = 0;
            context.First = sequence.Cast<object>().FirstOrDefault(); //TODO: don't enumerate multiple times
            context.Last = sequence.Cast<object>().LastOrDefault(); //TODO: don't enumerate multiple times
            foreach(object item in sequence)
            {
                template(context.TextWriter, item);
                context.Index++;
            }
            if(context.Index == 0)
            {
                ifEmpty(context.TextWriter, context.Value);
            }
        }

        private class IteratorBindingContext : BindingContext
        {
            public IteratorBindingContext(BindingContext context)
                : base(context.Value, context.TextWriter, context.ParentContext)
            {
            }

            public int Index { get; set; }

            public object First { get; set; }

            public object Last { get; set; }

            public override object GetContextVariable(string variableName)
            {
                object returnValue = null;
                if (string.Equals(variableName, "index", StringComparison.InvariantCultureIgnoreCase))
                {
                    returnValue = this.Index;
                }
                else if (string.Equals(variableName, "first", StringComparison.InvariantCultureIgnoreCase))
                {
                    returnValue = this.First;
                }
                else if (string.Equals(variableName, "last", StringComparison.InvariantCultureIgnoreCase))
                {
                    returnValue = this.Last;
                }
                return returnValue;
            }
        }
    }
}

