using System;
using System.Linq.Expressions;
using System.IO;
using System.Text;

namespace HandlebarsDotNet.Compiler
{
    internal class SubExpressionVisitor : HandlebarsExpressionVisitor
    {
        public static Expression Visit(Expression expr, CompilationContext context)
        {
            return new SubExpressionVisitor(context).Visit(expr);
        }

        private SubExpressionVisitor(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitSubExpression(SubExpressionExpression subex)
        {
            var helperCall = subex.Expression as MethodCallExpression;
            if (helperCall == null)
            {
                throw new HandlebarsCompilerException("Sub-expression does not contain a converted MethodCall expression");
            }
            return Expression.Call(
                new Func<HandlebarsHelper, object, object[], string>(CaptureTextWriterOutputFromHelper).Method,
                Expression.Constant((HandlebarsHelper)Delegate.CreateDelegate(typeof(HandlebarsHelper), helperCall.Method)),
                helperCall.Arguments[1],
                helperCall.Arguments[2]);
        }

        private static string CaptureTextWriterOutputFromHelper(
            HandlebarsHelper helper,
            object context,
            object[] arguments)
        {
            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder))
            {
                helper(writer, context, arguments);
            }
            return builder.ToString();
        }
    }
}

