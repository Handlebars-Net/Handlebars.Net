﻿using System;
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
            HandlebarsHelper helper = GetHelperDelegateFromMethodCallExpression(helperCall);
            return Expression.Call(
                new Func<HandlebarsHelper, object, object[], string>(CaptureTextWriterOutputFromHelper).Method,
                Expression.Constant(helper),
                helperCall.Arguments[1],
                helperCall.Arguments[2]);
        }

        private static HandlebarsHelper GetHelperDelegateFromMethodCallExpression(MethodCallExpression helperCall)
        {
            object target = helperCall.Object;
            HandlebarsHelper helper;
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
                helper = (HandlebarsHelper)Delegate.CreateDelegate(typeof(HandlebarsHelper), target, helperCall.Method);
            }
            else
            {
                helper = (HandlebarsHelper)Delegate.CreateDelegate(typeof(HandlebarsHelper), helperCall.Method);
            }
            return helper;
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

