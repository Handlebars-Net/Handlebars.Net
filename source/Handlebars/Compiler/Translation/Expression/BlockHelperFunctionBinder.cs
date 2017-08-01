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
            };


            //this is the hackiest part of the inline partial PR. This codebase looks like
            //it would take me a long time to figure out. Stuffing this here makes it work though.
            //Hopefully you have a better idea of where this should go
            if (bhex.HelperName == "#*inline")
            {   
                //This block is an inline block so we will compile it and register it as a partial template
                var args = bhex.Arguments.GetEnumerator();
                args.MoveNext();

                var partialName = ((ConstantExpression)args.Current).Value.ToString();

                var compiledPartial = fb.Compile(((BlockExpression)bhex.Body).Expressions, null);
                    
                CompilationContext.Configuration.RegisteredTemplates.AddOrUpdate(partialName, compiledPartial);
            }


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

