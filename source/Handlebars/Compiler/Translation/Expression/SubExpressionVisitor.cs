using System;
using System.Linq.Expressions;
using System.IO;
using System.Reflection;
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
            HandlebarsHelperWithName helper = GetHelperDelegateFromMethodCallExpression(helperCall);
            return Expression.Call(
#if netstandard
                new Func<HandlebarsHelperWithName, dynamic, string, object[], string>(CaptureTextWriterOutputFromHelper).GetMethodInfo(),
#else
                new Func<HandlebarsHelperWithName, dynamic, string, object[], string>(CaptureTextWriterOutputFromHelper).Method,
#endif
                Expression.Constant(helper),
                Visit(helperCall.Arguments[1]),
                Visit(helperCall.Arguments[2]),
                Visit(helperCall.Arguments[3]));
        }

        private static HandlebarsHelperWithName GetHelperDelegateFromMethodCallExpression(MethodCallExpression helperCall)
        {
            object target = helperCall.Object;
            HandlebarsHelperWithName helper;
            if (target != null)
            {
                if (target is ConstantExpression)
                {
                    target = ((ConstantExpression)target).Value;
                }
                else
                {
                    throw new NotSupportedException("Helper method instance target must be reduced to a ConstantExpression");
                }
#if netstandard
                helper = (HandlebarsHelperWithName)helperCall.Method.CreateDelegate(typeof(HandlebarsHelperWithName), target);
#else
                helper = (HandlebarsHelperWithName)Delegate.CreateDelegate(typeof(HandlebarsHelperWithName), target, helperCall.Method);
#endif
            }
            else
            {
#if netstandard
                helper = (HandlebarsHelperWithName)helperCall.Method.CreateDelegate(typeof(HandlebarsHelperWithName));
#else
                helper = (HandlebarsHelperWithName)Delegate.CreateDelegate(typeof(HandlebarsHelperWithName), helperCall.Method);
#endif
            }
            return helper;
        }

        private static string CaptureTextWriterOutputFromHelper(
            HandlebarsHelperWithName helper,
            dynamic context,
            string name,
            object[] arguments)
        {
            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder))
            {
                helper(writer, context, name, arguments);
            }
            return builder.ToString();
        }
    }
}

