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
        private static readonly LookupSlim<int, DeferredValue<int, ConstructorInfo>> ArgumentsConstructorsMap 
            = new LookupSlim<int, DeferredValue<int, ConstructorInfo>>();

        private static readonly Func<int, DeferredValue<int, ConstructorInfo>> CtorFactory = i =>
        {
            return new DeferredValue<int, ConstructorInfo>(i, count =>
            {
                var objectType = typeof(object);
                var argumentTypes = new Type[count];
                for (var index = 0; index < argumentTypes.Length; index++)
                {
                    argumentTypes[index] = objectType;
                }

                return typeof(Arguments).GetConstructor(argumentTypes);
            });
        };

        public static ExpressionContainer<Arguments> CreateArguments(IEnumerable<Expression> expressions, CompilationContext compilationContext)
        {
            var arguments = expressions
                .ApplyOn<Expression, PathExpression>(path => path.Context = PathExpression.ResolutionContext.Parameter)
                .Select(o => FunctionBuilder.Reduce(o, compilationContext))
                .ToArray();

            if (arguments.Length == 0) return New(() => new Arguments(0));

            var constructor = ArgumentsConstructorsMap.GetOrAdd(arguments.Length, CtorFactory).Value;
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