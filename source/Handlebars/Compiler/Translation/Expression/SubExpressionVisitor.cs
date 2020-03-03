using System.Linq.Expressions;
using System.IO;
using System.Linq;

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
            switch (subex.Expression)
            {
                case InvocationExpression invocationExpression:
                    return HandleInvocationExpression(invocationExpression);

                case MethodCallExpression callExpression:
                    return HandleMethodCallExpression(callExpression);
                
                default:
                    var expression = FunctionBuilder.Reduce(subex.Expression, CompilationContext);
                    if (expression is MethodCallExpression lateBoundCall)
                        return HandleMethodCallExpression(lateBoundCall);
                    
                    throw new HandlebarsCompilerException("Sub-expression does not contain a converted MethodCall expression");
            }
        }

        private Expression HandleMethodCallExpression(MethodCallExpression helperCall)
        {
            if (helperCall.Type != typeof(void))
            {
                return helperCall.Update(helperCall.Object, 
                    ExpressionShortcuts.ReplaceValuesOf<TextWriter>(helperCall.Arguments, ExpressionShortcuts.Null<TextWriter>()).Select(Visit)
                );
            }

            var writer = ExpressionShortcuts.Var<TextWriter>();
            helperCall = helperCall.Update(helperCall.Object, 
                ExpressionShortcuts.ReplaceValuesOf<TextWriter>(helperCall.Arguments, writer).Select(Visit)
            );
            
            return ExpressionShortcuts.Block()
                .Parameter(writer)
                .Line(Expression.Assign(writer, ExpressionShortcuts.New<PolledStringWriter>()))
                .Line(writer.Using(container =>
                {
                    var body = new BlockBody { helperCall };
                    body.AddRange(container.Call(o => o.ToString()).Return());
                    return body;
                }))
                .Invoke<object>();
        }

        private Expression HandleInvocationExpression(InvocationExpression invocation)
        {
            if (invocation.Type != typeof(void))
            {
                return invocation.Update(invocation.Expression, 
                    ExpressionShortcuts.ReplaceValuesOf<TextWriter>(invocation.Arguments, ExpressionShortcuts.Null<TextWriter>()).Select(Visit)
                );
            }

            var writer = ExpressionShortcuts.Var<TextWriter>();
            invocation = invocation.Update(invocation.Expression, 
                ExpressionShortcuts.ReplaceValuesOf<TextWriter>(invocation.Arguments, writer).Select(Visit)
            );
            
            return ExpressionShortcuts.Block()
                .Parameter(writer)
                .Line(Expression.Assign(writer, ExpressionShortcuts.New<PolledStringWriter>()))
                .Line(writer.Using(container =>
                {
                    var body = new BlockBody { invocation };
                    body.AddRange(container.Call(o => o.ToString()).Return());
                    return body;
                }))
                .Invoke<object>();
        }
    }
}

