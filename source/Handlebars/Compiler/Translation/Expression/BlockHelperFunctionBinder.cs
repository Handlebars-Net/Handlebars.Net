using System.Linq.Expressions;

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
            return sex.Body is BlockHelperExpression ? Visit(sex.Body) : sex;
        }

        protected override Expression VisitBlockHelperExpression(BlockHelperExpression bhex)
        {
            var isInlinePartial = bhex.HelperName == "#*inline";

            var fb = new FunctionBuilder(CompilationContext.Configuration);

            var context = E.Arg<BindingContext>(CompilationContext.BindingContext);
            var bindingContext = isInlinePartial 
                ? context.Cast<object>()
                : context.Property(o => o.Value);

            var blockParamsExpression = E.New(
                () => new BlockParamsValueProvider(context, E.Arg<BlockParam>(bhex.BlockParams))
            );
            
            var body = fb.Compile(((BlockExpression) bhex.Body).Expressions, context);
            var inversion = fb.Compile(((BlockExpression) bhex.Inversion).Expressions, context);
            var helper = CompilationContext.Configuration.BlockHelpers[bhex.HelperName.Replace("#", "")];
            
            var helperOptions = E.New(
                () => new HelperOptions(E.Arg(body), E.Arg(inversion), blockParamsExpression)
            );
            
            return E.Call(
                () => helper(context.Property(o => o.TextWriter), helperOptions, bindingContext, E.Array<object>(bhex.Arguments))
            );
        }
    }
}

