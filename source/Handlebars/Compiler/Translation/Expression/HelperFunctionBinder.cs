using System.Collections.Generic;
using System.Linq.Expressions;
using Expressions.Shortcuts;
using HandlebarsDotNet.Decorators;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.BlockHelpers;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.Runtime;
using static Expressions.Shortcuts.ExpressionShortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal class HelperFunctionBinder : HandlebarsExpressionVisitor
    {
        private readonly List<DecoratorDefinition> _decorators;
        private CompilationContext CompilationContext { get; }

        public HelperFunctionBinder(CompilationContext compilationContext, List<DecoratorDefinition> decorators)
        {
            _decorators = decorators;
            CompilationContext = compilationContext;
        }
        
        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            return sex.Body is HelperExpression ? Visit(sex.Body) : sex;
        }
        
        protected override Expression VisitHelperExpression(HelperExpression hex)
        {
            if (hex.HelperName.StartsWith("*"))
            {
                _decorators.Add(VisitDecoratorExpression(hex));
                return Expression.Empty();
            }
            
            var pathInfo = PathInfoStore.Current.GetOrAdd(hex.HelperName);
            if(!pathInfo.IsValidHelperLiteral && !CompilationContext.Configuration.Compatibility.RelaxedHelperNaming) return Expression.Empty();
            
            var bindingContext = CompilationContext.Args.BindingContext;
            var options = New(() => new HelperOptions(pathInfo, bindingContext));
            var textWriter = CompilationContext.Args.EncodedWriter;

            var contextValue = New(() => new Context(bindingContext));
            var args = FunctionBinderHelpers.CreateArguments(hex.Arguments, CompilationContext);

            var configuration = CompilationContext.Configuration;
            if (configuration.Helpers.TryGetValue(pathInfo, out var helper))
            {
                return Call(() => helper.Value.Invoke(textWriter, options, contextValue, args));
            }
            
            for (var index = 0; index < configuration.HelperResolvers.Count; index++)
            {
                var resolver = configuration.HelperResolvers[index];
                if (resolver.TryResolveHelper(pathInfo, typeof(object), out var resolvedHelper))
                {
                    helper = new Ref<IHelperDescriptor<HelperOptions>>(resolvedHelper);
                    configuration.Helpers.AddOrReplace(pathInfo, helper);
                    return Call(() => resolvedHelper.Invoke(textWriter, options, contextValue, args));
                }
            }

            var lateBindDescriptor = new Ref<IHelperDescriptor<HelperOptions>>(new LateBindHelperDescriptor(pathInfo));
            configuration.Helpers.AddOrReplace(pathInfo, lateBindDescriptor);
            
            return Call(() => lateBindDescriptor.Value.Invoke(textWriter, options, contextValue, args));
        }
        
        private DecoratorDefinition VisitDecoratorExpression(HelperExpression hex)
        {
            var pathInfo = PathInfoStore.Current.GetOrAdd(hex.HelperName);
            if(!pathInfo.IsValidHelperLiteral && !CompilationContext.Configuration.Compatibility.RelaxedHelperNaming) return new DecoratorDefinition();
            
            var bindingContext = CompilationContext.Args.BindingContext;
            var options = New(() => new DecoratorOptions(pathInfo, bindingContext));

            var contextValue = New(() => new Context(bindingContext));
            var args = FunctionBinderHelpers.CreateArguments(hex.Arguments, CompilationContext);

            var parameter = Parameter<TemplateDelegate>();
            var configuration = CompilationContext.Configuration;
            if (configuration.Decorators.TryGetValue(pathInfo, out var helper))
            {
                return new DecoratorDefinition(
                    Call(() => helper.Value.Invoke(parameter, options, contextValue, args)),
                    parameter
                );
            }
            
            var emptyDecorator = new Ref<IDecoratorDescriptor<DecoratorOptions>>(new EmptyDecorator(pathInfo));
            configuration.Decorators.AddOrReplace(pathInfo, emptyDecorator);
            
            return new DecoratorDefinition(
                Call(() => emptyDecorator.Value.Invoke(parameter, options, contextValue, args)),
                parameter
            );
        }
    }
}
