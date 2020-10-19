using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Expressions.Shortcuts;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Helpers;
using static Expressions.Shortcuts.ExpressionShortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal class HelperFunctionBinder : HandlebarsExpressionVisitor
    {
        private static readonly LookupSlim<int, DeferredValue<Expression[], ConstructorInfo>> ArgumentsConstructorsMap = new LookupSlim<int, DeferredValue<Expression[], ConstructorInfo>>();
        
        private CompilationContext CompilationContext { get; }

        public HelperFunctionBinder(CompilationContext compilationContext)
        {
            CompilationContext = compilationContext;
        }
        
        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            return sex.Body is HelperExpression ? Visit(sex.Body) : sex;
        }
        
        protected override Expression VisitHelperExpression(HelperExpression hex)
        {
            var pathInfo = CompilationContext.Configuration.PathInfoStore.GetOrAdd(hex.HelperName);
            if(!pathInfo.IsValidHelperLiteral && !CompilationContext.Configuration.Compatibility.RelaxedHelperNaming) return Expression.Empty();
            
            var helperName = pathInfo.TrimmedPath;
            var bindingContext = Arg<BindingContext>(CompilationContext.BindingContext);
            var textWriter = Arg<EncodedTextWriter>(CompilationContext.EncodedWriter);
            
            var contextValue = bindingContext.Property(o => o.Value);
            var args = CreateArguments();

            var configuration = CompilationContext.Configuration;
            if (configuration.Helpers.TryGetValue(pathInfo, out var helper))
            {
                return Call(() => helper.Value.WriteInvoke(bindingContext, textWriter, contextValue, args));
            }
            
            for (var index = 0; index < configuration.HelperResolvers.Count; index++)
            {
                var resolver = configuration.HelperResolvers[index];
                if (resolver.TryResolveHelper(helperName, typeof(object), out var resolvedHelper))
                {
                    helper = new StrongBox<HelperDescriptorBase>(resolvedHelper);
                    configuration.Helpers.Add(pathInfo, helper);
                    return Call(() => resolvedHelper.WriteInvoke(bindingContext, textWriter, contextValue, args));
                }
            }

            var lateBindDescriptor = new StrongBox<HelperDescriptorBase>(new LateBindHelperDescriptor(pathInfo, configuration));
            configuration.Helpers.Add(pathInfo, lateBindDescriptor);
            
            return Call(() => lateBindDescriptor.Value.WriteInvoke(bindingContext, textWriter, contextValue, args));
            
            ExpressionContainer<Arguments> CreateArguments()
            {
                Expression[] arguments = hex.Arguments
                    .ApplyOn<Expression, PathExpression>(path => path.Context = PathExpression.ResolutionContext.Parameter)
                    .Select(o => FunctionBuilder.Reduce(o, CompilationContext))
                    .ToArray();

                if (arguments.Length == 0)
                {
                    return Arg(Arguments.Empty);
                }

                var constructor = ArgumentsConstructorsMap.GetOrAdd(arguments.Length, (i, d) =>
                {
                    return new DeferredValue<Expression[], ConstructorInfo>(d, o =>
                    {
                        var objectType = typeof(object);
                        var argumentTypes = new Type[o.Length];
                        for (var index = 0; index < argumentTypes.Length; index++)
                        {
                            argumentTypes[index] = objectType;
                        }

                        return typeof(Arguments).GetConstructor(argumentTypes);
                    });
                }, arguments).Value;

                if (constructor == null) // cannot handle by direct args pass
                {
                    var arr = Array<object>(arguments);
                    return New(() => new Arguments(arr));
                }
                
                return Arg<Arguments>(Expression.New(constructor, arguments));
            }
        }
    }
}
