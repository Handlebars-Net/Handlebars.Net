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

            var context = ExpressionShortcuts.Arg<BindingContext>(CompilationContext.BindingContext);
            var bindingContext = isInlinePartial 
                ? context.Cast<object>()
                : context.Property(o => o.Value);

            var blockParamsExpression = ExpressionShortcuts.New(
                () => new BlockParamsValueProvider(context, ExpressionShortcuts.Arg<BlockParam>(bhex.BlockParams))
            );
            
            var body = fb.Compile(((BlockExpression) bhex.Body).Expressions, context);
            var inversion = fb.Compile(((BlockExpression) bhex.Inversion).Expressions, context);
            var helper = CompilationContext.Configuration.BlockHelpers[bhex.HelperName.Replace("#", "")];
            
            var helperOptions = ExpressionShortcuts.New(
                () => new HelperOptions(ExpressionShortcuts.Arg(body), ExpressionShortcuts.Arg(inversion), blockParamsExpression)
            );
            
            return ExpressionShortcuts.Call(
                () => helper(context.Property(o => o.TextWriter), helperOptions, bindingContext, ExpressionShortcuts.Array<object>(bhex.Arguments))
            );
        }
    }
}

