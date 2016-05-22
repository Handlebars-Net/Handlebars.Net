using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HandlebarsDotNet.Compiler
{
    internal class BlockHelperFunctionBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new BlockHelperFunctionBinder(context).Visit(expr);
        }

        private BlockHelperFunctionBinder(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            if (sex.Body is BlockHelperExpression)
            {
                return Visit(sex.Body);
            }
            else
            {
                return sex;
            }
        }

        protected override Expression VisitBlockHelperExpression(BlockHelperExpression bhex)
        {
            var fb = new FunctionBuilder(CompilationContext.Configuration);
            var body = fb.Compile(((BlockExpression)bhex.Body).Expressions, CompilationContext.BindingContext);
            var inversion = fb.Compile(((BlockExpression)bhex.Inversion).Expressions, CompilationContext.BindingContext);
            var helper = CompilationContext.Configuration.BlockHelpers[bhex.HelperName.Replace("#", "")];
            var arguments = new Expression[]
            {
#if netstandard
                Expression.Property(
                    CompilationContext.BindingContext,
                    typeof(BindingContext).GetRuntimeProperty("TextWriter")),
                Expression.New(
                        typeof(HelperOptions).GetTypeInfo().GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0],
                        body,
                        inversion),
                Expression.Property(
                    CompilationContext.BindingContext,
                    typeof(BindingContext).GetRuntimeProperty("Value")),
                Expression.NewArrayInit(typeof(object), bhex.Arguments)
#else
                Expression.Property(
                    CompilationContext.BindingContext,
                    typeof(BindingContext).GetProperty("TextWriter")),
                Expression.New(
                        typeof(HelperOptions).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0],
                        body,
                        inversion),
                Expression.Property(
                    CompilationContext.BindingContext,
                    typeof(BindingContext).GetProperty("Value")),
                Expression.NewArrayInit(typeof(object), bhex.Arguments)
#endif
            };
            if (helper.Target != null)
            {
                return Expression.Call(
                    Expression.Constant(helper.Target),
#if netstandard
                    helper.GetMethodInfo(),
#else
                    helper.Method,
#endif
                    arguments);
            }
            else
            {
                return Expression.Call(
#if netstandard
                    helper.GetMethodInfo(),
#else
                    helper.Method,
#endif
                    arguments);
            }
        }
    }
}

