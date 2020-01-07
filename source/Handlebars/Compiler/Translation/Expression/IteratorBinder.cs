using System;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace HandlebarsDotNet.Compiler
{
    internal class IteratorBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new IteratorBinder(context).Visit(expr);
        }

        private IteratorBinder(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitIteratorExpression(IteratorExpression iex)
        {
            var iteratorBindingContext = Expression.Variable(typeof(BindingContext), "context");
            var blockParamsValueBinder = Expression.Variable(typeof(BlockParamsValueProvider), "blockParams");
            var configuration = Expression.Constant(CompilationContext.Configuration);
            var ctor = typeof(BlockParamsValueProvider).GetConstructors().Single();

            return Expression.Block(
                new ParameterExpression[]
                {
                    iteratorBindingContext, blockParamsValueBinder
                },
                Expression.Assign(iteratorBindingContext, CompilationContext.BindingContext),
                Expression.Assign(blockParamsValueBinder, Expression.New(ctor, iteratorBindingContext, configuration, iex.BlockParams)),
                Expression.IfThenElse(
                    Expression.TypeIs(iex.Sequence, typeof(IEnumerable)),
                    Expression.IfThenElse(
#if netstandard
                        Expression.Call(new Func<object, bool>(IsNonListDynamic).GetMethodInfo(), new[] { iex.Sequence }),
#else
                        Expression.Call(new Func<object, bool>(IsNonListDynamic).Method, new[] { iex.Sequence }),
#endif
                        GetDynamicIterator(iteratorBindingContext, blockParamsValueBinder, iex),
                        Expression.IfThenElse(
#if netstandard
                            Expression.Call(new Func<object, bool>(IsGenericDictionary).GetMethodInfo(), new[] { iex.Sequence }),
#else
                            Expression.Call(new Func<object, bool>(IsGenericDictionary).Method, new[] { iex.Sequence }),
#endif
                            GetDictionaryIterator(iteratorBindingContext, blockParamsValueBinder, iex),
                            GetEnumerableIterator(iteratorBindingContext, blockParamsValueBinder, iex))),
                    GetObjectIterator(iteratorBindingContext, blockParamsValueBinder, iex))
            );
        }

        private Expression GetEnumerableIterator(Expression contextParameter, Expression blockParamsParameter, IteratorExpression iex)
        {
            var fb = new FunctionBuilder(CompilationContext.Configuration);
            return Expression.Call(
#if netstandard
                new Action<BindingContext, BlockParamsValueProvider, IEnumerable, Action<TextWriter, object>, Action<TextWriter, object>>(IterateEnumerable).GetMethodInfo(),
#else
                    new Action<BindingContext, BlockParamsValueProvider, IEnumerable, Action<TextWriter, object>, Action<TextWriter, object>>(IterateEnumerable).Method,
#endif
                new Expression[]
                {
                    contextParameter,
                    blockParamsParameter,
                    Expression.Convert(iex.Sequence, typeof(IEnumerable)),
                    fb.Compile(new [] { iex.Template }, contextParameter),
                    fb.Compile(new [] { iex.IfEmpty }, CompilationContext.BindingContext)
                });
        }

        private Expression GetObjectIterator(Expression contextParameter, Expression blockParamsParameter, IteratorExpression iex)
        {
            var fb = new FunctionBuilder(CompilationContext.Configuration);
            return Expression.Call(
#if netstandard
                new Action<BindingContext, BlockParamsValueProvider, object, Action<TextWriter, object>, Action<TextWriter, object>>(IterateObject).GetMethodInfo(),
#else
                    new Action<BindingContext, BlockParamsValueProvider, object, Action<TextWriter, object>, Action<TextWriter, object>>(IterateObject).Method,
#endif
                new[]
                {
                    contextParameter,
                    blockParamsParameter,
                    iex.Sequence,
                    fb.Compile(new [] { iex.Template }, contextParameter),
                    fb.Compile(new [] { iex.IfEmpty }, CompilationContext.BindingContext)
                });
        }

        private Expression GetDictionaryIterator(Expression contextParameter, Expression blockParamsParameter, IteratorExpression iex)
        {
            var fb = new FunctionBuilder(CompilationContext.Configuration);
            return Expression.Call(
#if netstandard
                new Action<BindingContext, BlockParamsValueProvider, IEnumerable, Action<TextWriter, object>, Action<TextWriter, object>>(IterateDictionary).GetMethodInfo(),
#else
                    new Action<BindingContext, BlockParamsValueProvider, IEnumerable, Action<TextWriter, object>, Action<TextWriter, object>>(IterateDictionary).Method,
#endif
                new[]
                {
                    contextParameter,
                    blockParamsParameter,
                    Expression.Convert(iex.Sequence, typeof(IEnumerable)),
                    fb.Compile(new[] {iex.Template}, contextParameter),
                    fb.Compile(new[] {iex.IfEmpty}, CompilationContext.BindingContext)
                });
        }

        private Expression GetDynamicIterator(Expression contextParameter, Expression blockParamsParameter, IteratorExpression iex)
        {
            var fb = new FunctionBuilder(CompilationContext.Configuration);
            return Expression.Call(
#if netstandard
                new Action<BindingContext, BlockParamsValueProvider, IDynamicMetaObjectProvider, Action<TextWriter, object>,Action<TextWriter, object>>(IterateDynamic).GetMethodInfo(),
#else
                    new Action<BindingContext, BlockParamsValueProvider, IDynamicMetaObjectProvider, Action<TextWriter, object>, Action<TextWriter, object>>(IterateDynamic).Method,
#endif
                new[]
                {
                    contextParameter,
                    blockParamsParameter,
                    Expression.Convert(iex.Sequence, typeof(IDynamicMetaObjectProvider)),
                    fb.Compile(new[] {iex.Template}, contextParameter),
                    fb.Compile(new[] {iex.IfEmpty}, CompilationContext.BindingContext)
                });
        }

        private static bool IsNonListDynamic(object target)
        {
            var interfaces = target.GetType().GetInterfaces();
            return interfaces.Contains(typeof(IDynamicMetaObjectProvider))
                && ((IDynamicMetaObjectProvider)target).GetMetaObject(Expression.Constant(target)).GetDynamicMemberNames().Any();
        }

        private static bool IsGenericDictionary(object target)
        {
            return
                target.GetType()
#if netstandard
                    .GetInterfaces()
                    .Where(i => i.GetTypeInfo().IsGenericType)

#else
                    .GetInterfaces()
                    .Where(i => i.IsGenericType)
#endif
                    .Any(i => i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }

        private static void IterateObject(
            BindingContext context,
            BlockParamsValueProvider blockParamsValueProvider,
            object target,
            Action<TextWriter, object> template,
            Action<TextWriter, object> ifEmpty)
        {
            if (HandlebarsUtils.IsTruthy(target))
            {
                var objectEnumerator = new ObjectEnumeratorValueProvider();
                context.RegisterValueProvider(objectEnumerator);
                blockParamsValueProvider.Configure((parameters, binder) =>
                {
                    binder(parameters.Keys.ElementAtOrDefault(0), () => objectEnumerator.Key);
                    binder(parameters.Keys.ElementAtOrDefault(1), () => objectEnumerator.Value);
                    binder(parameters.Keys.ElementAtOrDefault(2), () => target);
                });

                objectEnumerator.Index = 0;
                var targetType = target.GetType();
                var properties = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public).OfType<MemberInfo>();
                var fields = targetType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var enumerableValue in new ExtendedEnumerable<MemberInfo>(properties.Concat(fields)))
                {
                    var member = enumerableValue.Value;
                    objectEnumerator.Key = member.Name;
                    objectEnumerator.Value = AccessMember(target, member);
                    objectEnumerator.First = enumerableValue.IsFirst;
                    objectEnumerator.Last = enumerableValue.IsLast;
                    objectEnumerator.Index = enumerableValue.Index;

                    template(context.TextWriter, objectEnumerator.Value);
                }

                if (objectEnumerator.Index == 0)
                {
                    ifEmpty(context.TextWriter, context.Value);
                }
            }
            else
            {
                ifEmpty(context.TextWriter, context.Value);
            }
        }

        private static void IterateDictionary(
            BindingContext context,
            BlockParamsValueProvider blockParamsValueProvider,
            IEnumerable target,
            Action<TextWriter, object> template,
            Action<TextWriter, object> ifEmpty)
        {
            if (HandlebarsUtils.IsTruthy(target))
            {
                var objectEnumerator = new ObjectEnumeratorValueProvider();
                context.RegisterValueProvider(objectEnumerator);
                blockParamsValueProvider.Configure((parameters, binder) =>
                {
                    binder(parameters.Keys.ElementAtOrDefault(0), () => objectEnumerator.Key);
                    binder(parameters.Keys.ElementAtOrDefault(1), () => objectEnumerator.Value);
                    binder(parameters.Keys.ElementAtOrDefault(2), () => target);
                });
                
                objectEnumerator.Index = 0;
                var targetType = target.GetType();
#if netstandard
                var keysProperty = targetType.GetRuntimeProperty("Keys");
#else
                var keysProperty = targetType.GetProperty("Keys");
#endif
                if (keysProperty?.GetGetMethod().Invoke(target, null) is IEnumerable keys)
                {
                    var getItemMethodInfo = targetType.GetMethod("get_Item");
                    var parameters = new object[1];
                    foreach (var enumerableValue in new ExtendedEnumerable<object>(keys))
                    {
                        var key = parameters[0] = enumerableValue.Value;
                        objectEnumerator.Key = key.ToString();
                        objectEnumerator.Value = getItemMethodInfo.Invoke(target, parameters);
                        objectEnumerator.First = enumerableValue.IsFirst;
                        objectEnumerator.Last = enumerableValue.IsLast;
                        objectEnumerator.Index = enumerableValue.Index;

                        template(context.TextWriter, objectEnumerator.Value);
                    }
                }
                if (objectEnumerator.Index == 0)
                {
                    ifEmpty(context.TextWriter, context.Value);
                }
            }
            else
            {
                ifEmpty(context.TextWriter, context.Value);
            }
        }

        private static void IterateDynamic(
            BindingContext context,
            BlockParamsValueProvider blockParamsValueProvider,
            IDynamicMetaObjectProvider target,
            Action<TextWriter, object> template,
            Action<TextWriter, object> ifEmpty)
        {
            if (HandlebarsUtils.IsTruthy(target))
            {
                var objectEnumerator = new ObjectEnumeratorValueProvider();
                context.RegisterValueProvider(objectEnumerator);
                blockParamsValueProvider.Configure((parameters, binder) =>
                {
                    binder(parameters.Keys.ElementAtOrDefault(0), () => objectEnumerator.Key);
                    binder(parameters.Keys.ElementAtOrDefault(1), () => objectEnumerator.Value);
                    binder(parameters.Keys.ElementAtOrDefault(2), () => target);
                });
                
                objectEnumerator.Index = 0;
                var meta = target.GetMetaObject(Expression.Constant(target));
                foreach (var enumerableValue in new ExtendedEnumerable<string>(meta.GetDynamicMemberNames()))
                {
                    var name = enumerableValue.Value;
                    objectEnumerator.Key = name;
                    objectEnumerator.Value = GetProperty(target, name);
                    objectEnumerator.First = enumerableValue.IsFirst;
                    objectEnumerator.Last = enumerableValue.IsLast;
                    objectEnumerator.Index = enumerableValue.Index;

                    template(context.TextWriter, objectEnumerator.Value);
                }

                if (objectEnumerator.Index == 0)
                {
                    ifEmpty(context.TextWriter, context.Value);
                }
            }
            else
            {
                ifEmpty(context.TextWriter, context.Value);
            }
        }

        private static void IterateEnumerable(
            BindingContext context,
            BlockParamsValueProvider blockParamsValueProvider,
            IEnumerable sequence,
            Action<TextWriter, object> template,
            Action<TextWriter, object> ifEmpty)
        {
            var iterator = new IteratorValueProvider();
            context.RegisterValueProvider(iterator);
            blockParamsValueProvider.Configure((parameters, binder) =>
            {
                binder(parameters.Keys.ElementAtOrDefault(0), () => iterator.Index);
                binder(parameters.Keys.ElementAtOrDefault(1), () => iterator.Value);
                binder(parameters.Keys.ElementAtOrDefault(2), () => sequence);
            });
            
            iterator.Index = 0;
            foreach (var enumeratorValue in new ExtendedEnumerable<object>(sequence))
            {
                iterator.Value = enumeratorValue.Value;
                iterator.First = enumeratorValue.IsFirst;
                iterator.Last = enumeratorValue.IsLast;
                iterator.Index = enumeratorValue.Index;

                template(context.TextWriter, iterator.Value);
            }

            if (iterator.Index == 0)
            {
                ifEmpty(context.TextWriter, context.Value);
            }
        }

        private static object GetProperty(object target, string name)
        {
            var site = System.Runtime.CompilerServices.CallSite<Func<System.Runtime.CompilerServices.CallSite, object, object>>.Create(
                Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, name, target.GetType(), new[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) }));
            return site.Target(site, target);
        }

        private class IteratorValueProvider : IValueProvider
        {
            public object Value { get; set; }
            
            public int Index { get; set; }

            public bool First { get; set; }

            public bool Last { get; set; }

            public bool ProvidesNonContextVariables { get; } = false;

            public virtual bool TryGetValue(string memberName, out object value)
            {
                switch (memberName.ToLowerInvariant())
                {
                    case "index": 
                        value = Index;
                        return true;
                    case "first": 
                        value = First;
                        return true;
                    case "last": 
                        value = Last;
                        return true;
                    case "value": 
                        value = Value;
                        return true;
                    
                    default:
                        value = null;
                        return false;
                }
            }
        }
        
        private class ObjectEnumeratorValueProvider : IteratorValueProvider
        {
            public string Key { get; set; }

            public override bool TryGetValue(string memberName, out object value)
            {
                switch (memberName.ToLowerInvariant())
                {
                    case "key":
                        value = Key;
                        return true;
                    
                    default:
                        return base.TryGetValue(memberName, out value);
                }
            }
        }

        private static object AccessMember(object instance, MemberInfo member)
        {
            switch (member)
            {
                case PropertyInfo propertyInfo:
                    return propertyInfo.GetValue(instance, null);
                case FieldInfo fieldInfo:
                    return fieldInfo.GetValue(instance);
                default:
                    throw new InvalidOperationException("Requested member was not a field or property");
            }
        }
    }
}

