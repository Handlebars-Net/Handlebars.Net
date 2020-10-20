using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Expressions.Shortcuts;
using HandlebarsDotNet.Collections;
using static Expressions.Shortcuts.ExpressionShortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal static class FunctionBinderHelpers
    {
        private static readonly LookupSlim<int, DeferredValue<Expression[], ConstructorInfo>> ArgumentsConstructorsMap 
            = new LookupSlim<int, DeferredValue<Expression[], ConstructorInfo>>();

        private static readonly Func<Expression[], ConstructorInfo> ConstructorFactory = o =>
        {
            var objectType = typeof(object);
            var argumentTypes = new Type[o.Length];
            for (var index = 0; index < argumentTypes.Length; index++)
            {
                argumentTypes[index] = objectType;
            }

            return typeof(Arguments).GetConstructor(argumentTypes);
        };

        public static ExpressionContainer<Arguments> CreateArguments(IEnumerable<Expression> expressions, CompilationContext compilationContext)
        {
            var arguments = expressions
                .ApplyOn<Expression, PathExpression>(path => path.Context = PathExpression.ResolutionContext.Parameter)
                .Select(o => FunctionBuilder.Reduce(o, compilationContext))
                .ToArray();

            if (arguments.Length == 0) return Arg(Arguments.Empty);

            var deferredValue = ArgumentsConstructorsMap.GetOrAdd(arguments.Length, 
    (i, d) => new DeferredValue<Expression[], ConstructorInfo>(d, ConstructorFactory), arguments
            );

            var constructor = deferredValue.Value;
            if (!ReferenceEquals(constructor, null))
            {
                var newExpression = Expression.New(constructor, arguments);
                return Arg<Arguments>(newExpression);
            }
            
            var arr = Array<object>(arguments);
            return New(() => new Arguments(arr));
        }
    }
}