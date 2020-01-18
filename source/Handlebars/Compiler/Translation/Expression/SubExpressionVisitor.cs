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
                    var fb = new FunctionBuilder(CompilationContext.Configuration);
                    var expression = fb.Reduce(subex.Expression, CompilationContext);
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
                    E.ReplaceValuesOf<TextWriter>(helperCall.Arguments, E.Null<TextWriter>()).Select(Visit)
                );
            }

            var writer = E.Var<TextWriter>();
            helperCall = helperCall.Update(helperCall.Object, 
                E.ReplaceValuesOf<TextWriter>(helperCall.Arguments, writer).Select(Visit)
            );

            return E.Block()
                .Parameter(writer)
                .Line(Expression.Assign(writer, E.New<StringWriter>()))
                .Line(helperCall)
                .Line(writer.Call(w => w.ToString()))
                .Invoke<object>();
        }

        private Expression HandleInvocationExpression(InvocationExpression invocation)
        {
            if (invocation.Type != typeof(void))
            {
                return invocation.Update(invocation.Expression, 
                    E.ReplaceValuesOf<TextWriter>(invocation.Arguments, E.Null<TextWriter>()).Select(Visit)
                );
            }

            var writer = E.Var<TextWriter>();
            invocation = invocation.Update(invocation.Expression, 
                E.ReplaceValuesOf<TextWriter>(invocation.Arguments, writer).Select(Visit)
            );
            
            return E.Block()
                .Parameter(writer)
                .Line(Expression.Assign(writer, E.New<StringWriter>()))
                .Line(invocation)
                .Line(writer.Call(w => w.ToString()))
                .Invoke<object>();
        }
    }
}

