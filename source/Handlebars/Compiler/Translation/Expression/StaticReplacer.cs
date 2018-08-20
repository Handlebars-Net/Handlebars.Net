using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

        protected override Expression VisitStaticExpression(StaticExpression stex)
        {
	        var encodedTextWriter = Expression.Property(CompilationContext.BindingContext, "TextWriter");
#if netstandard
            var writeMethod = typeof(EncodedTextWriter).GetRuntimeMethod("Write", new [] { typeof(string), typeof(bool) });
#else
            var writeMethod = typeof(EncodedTextWriter).GetMethod("Write", new [] { typeof(string), typeof(bool) });
#endif

            return Expression.Call(encodedTextWriter, writeMethod, Expression.Constant(stex.Value), Expression.Constant(false));
        }
    }
}

