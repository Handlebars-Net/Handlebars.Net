using System;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Collections;
using System.Reflection;

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
            var iteratorBindingContext = Expression.Variable(typeof(BindingContext));
            return Expression.Block(
                new ParameterExpression[] {
                    iteratorBindingContext
                },
                Expression.IfThenElse(
                    Expression.NotEqual(Expression.TypeAs(iex.Sequence, typeof(IEnumerable)), Expression.Constant(null)),
                    GetEnumerableIterator(iteratorBindingContext, iex),
                    GetObjectIterator(iteratorBindingContext, iex))
                );
        }

        private Expression GetEnumerableIterator(Expression contextParameter, IteratorExpression iex)
        {
            var fb = new FunctionBuilder(_context.Configuration);
            return Expression.Block(
                Expression.Assign(contextParameter,
                    Expression.New(
                        typeof(IteratorBindingContext).GetConstructor(new[] { typeof(BindingContext) }),
                        new Expression[] { _context.BindingContext })),
                Expression.Call(
                    new Action<IteratorBindingContext, IEnumerable, Action<TextWriter, object>, Action<TextWriter, object>>(Iterate).Method,
                    new Expression[] {
                        Expression.Convert(contextParameter, typeof(IteratorBindingContext)),
                        Expression.Convert(iex.Sequence, typeof(IEnumerable)),
                        fb.Compile(new [] { iex.Template }, contextParameter),
                        fb.Compile(new [] { iex.IfEmpty }, _context.BindingContext) 
                    }));
        }

        private Expression GetObjectIterator(Expression contextParameter, IteratorExpression iex)
        {
            var fb = new FunctionBuilder(_context.Configuration);
            return Expression.Block(
                Expression.Assign(contextParameter,
                    Expression.New(
                        typeof(ObjectEnumeratorBindingContext).GetConstructor(new[] { typeof(BindingContext) }),
                        new Expression[] { _context.BindingContext })),
                Expression.Call(
                    new Action<ObjectEnumeratorBindingContext, object, Action<TextWriter, object>, Action<TextWriter, object>>(Iterate).Method,
                    new Expression[] {
                        Expression.Convert(contextParameter, typeof(ObjectEnumeratorBindingContext)),
                        iex.Sequence,
                        fb.Compile(new [] { iex.Template }, contextParameter),
                        fb.Compile(new [] { iex.IfEmpty }, _context.BindingContext) 
                    }));
        }

        private static void Iterate(
            ObjectEnumeratorBindingContext context,
            object target,
            Action<TextWriter, object> template,
            Action<TextWriter, object> ifEmpty)
        {
            bool firstSet = false;
            foreach(MemberInfo member in target.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public).OfType<MemberInfo>()
                .Concat(
                    target.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                ))
            {
                context.Key = member.Name;
                var value = AccessMember(target, member);
                if(firstSet == false)
                {
                    context.First = value;
                }
                template(context.TextWriter, value);
            }
            if(firstSet == false)
            {
                ifEmpty(context.TextWriter, context.Value);
            }
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
        }

        private class ObjectEnumeratorBindingContext : BindingContext
        {
            public ObjectEnumeratorBindingContext(BindingContext context)
                : base(context.Value, context.TextWriter, context.ParentContext)
            {
            }

            public string Key { get;set; }

            public object First { get; set; }
        }

        private static object AccessMember(object instance, MemberInfo member)
        {
            if(member.MemberType == System.Reflection.MemberTypes.Property)
            {
                return ((PropertyInfo)member).GetValue(instance, null);
            }
            else if(member.MemberType == System.Reflection.MemberTypes.Field)
            {
                return ((FieldInfo)member).GetValue(instance);
            }
            throw new InvalidOperationException("Requested member was not a field or property");
        }
    }
}

