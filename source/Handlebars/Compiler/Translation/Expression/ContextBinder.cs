using System;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal static class ContextBinder
    {
        public static Expression<TemplateDelegate> Bind(CompilationContext context, Expression body)
        {
            var blockExpression = (BlockExpression) body;
            
            return Expression.Lambda<TemplateDelegate>(
                blockExpression, 
                context.EncodedWriter, 
                context.BindingContext
            );
        }
    }
}