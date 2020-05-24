using System;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Expressions.Shortcuts;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.ObjectDescriptors;
using HandlebarsDotNet.Polyfills;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet.Compiler
{
    internal class IteratorBinder : HandlebarsExpressionVisitor
    {
        private CompilationContext CompilationContext { get; }

        public IteratorBinder(CompilationContext compilationContext)
        {
            CompilationContext = compilationContext;
        }
        
        protected override Expression VisitIteratorExpression(IteratorExpression iex)
        {
            var context = ExpressionShortcuts.Arg<BindingContext>(CompilationContext.BindingContext);
            var sequence = ExpressionShortcuts.Var<object>("sequence");
            
            var template = FunctionBuilder.CompileCore(new[] {iex.Template}, CompilationContext.Configuration);
            var ifEmpty = FunctionBuilder.CompileCore(new[] {iex.IfEmpty}, CompilationContext.Configuration);
            
            var compiledSequence = ExpressionShortcuts.Arg<object>(FunctionBuilder.Reduce(iex.Sequence, CompilationContext));
            var blockParams = ExpressionShortcuts.Arg<BlockParam>(iex.BlockParams);
            var blockParamsProvider = ExpressionShortcuts.Call(() => BlockParamsValueProvider.Create(context, blockParams));
            var blockParamsProviderVar = ExpressionShortcuts.Var<BlockParamsValueProvider>();

            return ExpressionShortcuts.Block()
                .Parameter(sequence, compiledSequence)
                .Parameter(blockParamsProviderVar, blockParamsProvider)
                .Line(blockParamsProviderVar.Using((self, builder) =>
                {
                    builder
                        .Line(ExpressionShortcuts.Call(() =>
                            Iterator.Iterate(context, self, sequence, template, ifEmpty)
                        ));
                }));
        }
    }

    internal static class Iterator
    {
        private static readonly ConfigureBlockParams BlockParamsEnumerableConfiguration = (parameters, binder, deps) =>
        {
            binder(parameters.ElementAtOrDefault(0), ctx => ctx.As<IteratorValueProvider>().Value, deps[0]);
            binder(parameters.ElementAtOrDefault(1), ctx => ctx.As<IteratorValueProvider>().Index, deps[0]);
        };

        private static readonly ConfigureBlockParams BlockParamsObjectEnumeratorConfiguration = (parameters, binder, deps) =>
        {
            binder(parameters.ElementAtOrDefault(0), ctx => ctx.As<ObjectEnumeratorValueProvider>().Value, deps[0]);
            binder(parameters.ElementAtOrDefault(1), ctx => ctx.As<ObjectEnumeratorValueProvider>().Key, deps[0]);
        };

        public static void Iterate(BindingContext context,
            BlockParamsValueProvider blockParamsValueProvider,
            object target,
            Action<BindingContext, TextWriter, object> template,
            Action<BindingContext, TextWriter, object> ifEmpty)
        {
            if (!HandlebarsUtils.IsTruthy(target))
            {
                ifEmpty(context, context.TextWriter, context.Value);
                return;
            }

            var targetType = target.GetType();
            if (!(context.Configuration.ObjectDescriptorProvider.CanHandleType(targetType) &&
                  context.Configuration.ObjectDescriptorProvider.TryGetDescriptor(targetType, out var descriptor)))
            {
                ifEmpty(context, context.TextWriter, context.Value);
                return;
            }

            if (!descriptor.ShouldEnumerate)
            {
                var properties = descriptor.GetProperties(descriptor, target);
                if (properties is IList propertiesList)
                {
                    IterateObjectWithStaticProperties(context, descriptor, blockParamsValueProvider, target, propertiesList, targetType, template, ifEmpty);
                    return;   
                }
                
                IterateObject(context, descriptor, blockParamsValueProvider, target, properties, targetType, template, ifEmpty);
                return;
            }

            if (target is IList list)
            {
                IterateList(context, blockParamsValueProvider, list, template, ifEmpty);
                return;
            }

            IterateEnumerable(context, blockParamsValueProvider, (IEnumerable) target, template, ifEmpty);
        }
        
        private static void IterateObject(BindingContext context,
            ObjectDescriptor descriptor,
            BlockParamsValueProvider blockParamsValueProvider,
            object target,
            IEnumerable<object> properties,
            Type targetType,
            Action<BindingContext, TextWriter, object> template,
            Action<BindingContext, TextWriter, object> ifEmpty)
        {
            using(var iterator = ObjectEnumeratorValueProvider.Create(context.Configuration))
            {
                blockParamsValueProvider?.Configure(BlockParamsObjectEnumeratorConfiguration, iterator);

                iterator.Index = 0;
                var accessor = descriptor.MemberAccessor;
                var enumerable = new ExtendedEnumerable<object>(properties);
                bool enumerated = false;

                foreach (var enumerableValue in enumerable)
                {
                    enumerated = true;
                    iterator.Key = enumerableValue.Value.ToString();
                    var key = iterator.Key.Intern();
                    iterator.Value = accessor.TryGetValue(target, targetType, key, out var value) ? value : null;
                    iterator.First = enumerableValue.IsFirst;
                    iterator.Last = enumerableValue.IsLast;
                    iterator.Index = enumerableValue.Index;

                    using(var innerContext = context.CreateChildContext(iterator.Value))
                    {
                        innerContext.RegisterValueProvider(blockParamsValueProvider);
                        innerContext.RegisterValueProvider(iterator);
                        template(context, context.TextWriter, innerContext);
                    }
                }

                if (iterator.Index == 0 && !enumerated)
                {
                    ifEmpty(context, context.TextWriter, context.Value);
                }
            }
        }
        
        private static void IterateObjectWithStaticProperties(BindingContext context,
            ObjectDescriptor descriptor,
            BlockParamsValueProvider blockParamsValueProvider,
            object target,
            IList properties,
            Type targetType,
            Action<BindingContext, TextWriter, object> template,
            Action<BindingContext, TextWriter, object> ifEmpty)
        {
            using(var iterator = ObjectEnumeratorValueProvider.Create(context.Configuration))
            {
                blockParamsValueProvider.Configure(BlockParamsObjectEnumeratorConfiguration, iterator);
                
                var accessor = descriptor.MemberAccessor;

                var count = properties.Count;
                for (iterator.Index = 0; iterator.Index < count; iterator.Index++)
                {
                    iterator.Key = properties[iterator.Index].ToString();
                    iterator.Value = accessor.TryGetValue(target, targetType, iterator.Key, out var value) ? value : null;
                    iterator.First = iterator.Index == 0;
                    iterator.Last = iterator.Index == count - 1;

                    using (var innerContext = context.CreateChildContext(iterator.Value))
                    {
                        innerContext.RegisterValueProvider(blockParamsValueProvider);
                        innerContext.RegisterValueProvider(iterator);
                        template(context, context.TextWriter, innerContext);
                    }
                }

                if (iterator.Index == 0)
                {
                    ifEmpty(context, context.TextWriter, context.Value);
                }
            }
        }
        
        private static void IterateList(BindingContext context,
            BlockParamsValueProvider blockParamsValueProvider,
            IList target,
            Action<BindingContext, TextWriter, object> template,
            Action<BindingContext, TextWriter, object> ifEmpty)
        {
            using (var iterator = IteratorValueProvider.Create())
            {
                blockParamsValueProvider?.Configure(BlockParamsEnumerableConfiguration, iterator);

                var count = target.Count;
                for (iterator.Index = 0; iterator.Index < count; iterator.Index++)
                {
                    iterator.Value = target[iterator.Index];
                    iterator.First = iterator.Index == 0;
                    iterator.Last = iterator.Index == count - 1;

                    using(var innerContext = context.CreateChildContext(iterator.Value))
                    {
                        innerContext.RegisterValueProvider(blockParamsValueProvider);
                        innerContext.RegisterValueProvider(iterator);
                        template(context, context.TextWriter, innerContext);
                    }   
                }

                if (iterator.Index == 0)
                {
                    ifEmpty(context, context.TextWriter, context.Value);
                }
            }
        }
        
        private static void IterateEnumerable(BindingContext context,
            BlockParamsValueProvider blockParamsValueProvider,
            IEnumerable target,
            Action<BindingContext, TextWriter, object> template,
            Action<BindingContext, TextWriter, object> ifEmpty)
        {
            using (var iterator = IteratorValueProvider.Create())
            {
                blockParamsValueProvider?.Configure(BlockParamsEnumerableConfiguration, iterator);

                iterator.Index = 0;
                var enumerable = new ExtendedEnumerable<object>(target);
                bool enumerated = false;
                
                foreach (var enumerableValue in enumerable)
                {
                    enumerated = true;
                    iterator.Value = enumerableValue.Value;
                    iterator.First = enumerableValue.IsFirst;
                    iterator.Last = enumerableValue.IsLast;

                    using(var innerContext = context.CreateChildContext(iterator.Value))
                    {
                        innerContext.RegisterValueProvider(blockParamsValueProvider);
                        innerContext.RegisterValueProvider(iterator);
                        template(context, context.TextWriter, innerContext);
                    }
                }

                if (iterator.Index == 0 && !enumerated)
                {
                    ifEmpty(context, context.TextWriter, context.Value);
                }
            }
        }
    }
}

