using System.Collections.Generic;
using System.Linq.Expressions;
using Expressions.Shortcuts;

namespace HandlebarsDotNet.Compiler
{
    public delegate TemplateDelegate DecoratorDelegate(in EncodedTextWriter writer, BindingContext context, TemplateDelegate function);
    
    internal readonly struct DecoratorDefinition
    {
        public DecoratorDefinition(Expression decorator, ExpressionContainer<TemplateDelegate> function)
        {
            Decorator = decorator;
            Function = function;
        }

        public Expression Decorator { get; }
        
        public ExpressionContainer<TemplateDelegate> Function { get; }
        
        public DecoratorDelegate Compile(CompilationContext context)
        {
            if (Function is null || Decorator is null) return (in EncodedTextWriter writer, BindingContext bindingContext, TemplateDelegate function) => function;

            var lambda = Expression.Lambda<DecoratorDelegate>(
                Decorator, 
                context.EncodedWriter, 
                context.BindingContext,
                Function.Expression as ParameterExpression
            );
            
            return context.Configuration.ExpressionCompiler.Compile(lambda);
        }
    }

    internal static class DecoratorDefinitionsExtensions
    {
        public static DecoratorDelegate Compile(
            this IReadOnlyList<DecoratorDefinition> decoratorDefinitions,
            CompilationContext context
        )
        {
            var decorator = decoratorDefinitions[0].Compile(context);
            
            for (var index = 1; index < decoratorDefinitions.Count; index++)
            {
                var definition = decoratorDefinitions[index];
                var f = definition.Compile(context);
                var current = decorator;
                decorator = (in EncodedTextWriter writer, BindingContext bindingContext, TemplateDelegate function) => 
                    f(writer, bindingContext, current(writer, bindingContext, function));
            }

            return decorator;
        }
    }
}