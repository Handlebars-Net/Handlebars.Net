using System;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
            var fb = new FunctionBuilder(CompilationContext.Configuration);
            
            var iteratorBindingContext = E.Var<BindingContext>("context");
            var blockParamsValueBinder = E.Var<BlockParamsValueProvider>("blockParams");
            var sequence = E.Var<object>("sequence");
            
            var template = E.Arg(fb.Compile(new[] {iex.Template}, iteratorBindingContext));
            var ifEmpty = E.Arg(fb.Compile(new[] {iex.IfEmpty}, CompilationContext.BindingContext));
            
            var context = CompilationContext.BindingContext;
            var compiledSequence = fb.Reduce(iex.Sequence, CompilationContext);
            var blockParamsProvider = E.New(() => new BlockParamsValueProvider(iteratorBindingContext, E.Arg<BlockParam>(iex.BlockParams)));


            return E.Block()
                .Parameter(iteratorBindingContext, context)
                .Parameter(blockParamsValueBinder, blockParamsProvider)
                .Parameter(sequence, compiledSequence)
                .Line(Expression.IfThenElse(
                    sequence.Is<IEnumerable>(),
                    Expression.IfThenElse(
                        E.Call(() => IsGenericDictionary(sequence)),
                        GetDictionaryIterator(iteratorBindingContext, blockParamsValueBinder, sequence, template, ifEmpty),
                        Expression.IfThenElse(
                            E.Call(() => IsNonListDynamic(sequence)),
                            GetDynamicIterator(iteratorBindingContext, blockParamsValueBinder, sequence, template, ifEmpty),
                            GetEnumerableIterator(iteratorBindingContext, blockParamsValueBinder, sequence, template, ifEmpty))),
                    GetObjectIterator(iteratorBindingContext, blockParamsValueBinder, sequence, template, ifEmpty)));
        }

        private Expression GetEnumerableIterator(
            ExpressionContainer<BindingContext> contextParameter, 
            ExpressionContainer<BlockParamsValueProvider> blockParamsParameter, 
            ExpressionContainer<object> sequence, 
            ExpressionContainer<Action<TextWriter, object>> template, 
            ExpressionContainer<Action<TextWriter, object>> ifEmpty
        )
        {    
            return E.Call(
                () => IterateEnumerable(contextParameter, blockParamsParameter, (IEnumerable) sequence, template, ifEmpty)
                );
        }

        private Expression GetObjectIterator(
            ExpressionContainer<BindingContext> contextParameter, 
            ExpressionContainer<BlockParamsValueProvider> blockParamsParameter, 
            ExpressionContainer<object> sequence, 
            ExpressionContainer<Action<TextWriter, object>> template, 
            ExpressionContainer<Action<TextWriter, object>> ifEmpty
        )
        {
            return E.Call(
                () => IterateObject(contextParameter, blockParamsParameter, sequence, template, ifEmpty)
                );
        }

        private Expression GetDictionaryIterator(
            ExpressionContainer<BindingContext> contextParameter, 
            ExpressionContainer<BlockParamsValueProvider> blockParamsParameter, 
            ExpressionContainer<object> sequence, 
            ExpressionContainer<Action<TextWriter, object>> template, 
            ExpressionContainer<Action<TextWriter, object>> ifEmpty
        )
        {
            return E.Call(
                () => IterateDictionary(contextParameter, blockParamsParameter, (IEnumerable) sequence, template, ifEmpty)
                );
        }

        private Expression GetDynamicIterator(
            ExpressionContainer<BindingContext> contextParameter, 
            ExpressionContainer<BlockParamsValueProvider> blockParamsParameter, 
            ExpressionContainer<object> sequence, 
            ExpressionContainer<Action<TextWriter, object>> template, 
            ExpressionContainer<Action<TextWriter, object>> ifEmpty)
        {
            return E.Call(
                () => IterateDynamic(contextParameter, blockParamsParameter, (IDynamicMetaObjectProvider) sequence, template, ifEmpty)
                );
        }

        private static bool IsNonListDynamic(object target)
        {
            if (target is IDynamicMetaObjectProvider metaObjectProvider)
            {
                return metaObjectProvider.GetMetaObject(Expression.Constant(target)).GetDynamicMemberNames().Any();
            }
            
            return false;
        }

        private static bool IsGenericDictionary(object target)
        {
            return
                target.GetType()
                    .GetInterfaces()
                    .Where(i => i.GetTypeInfo().IsGenericType)
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
                    binder(parameters.ElementAtOrDefault(0), () => objectEnumerator.Value);
                    binder(parameters.ElementAtOrDefault(1), () => objectEnumerator.Key);
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
                    binder(parameters.ElementAtOrDefault(0), () => objectEnumerator.Value);
                    binder(parameters.ElementAtOrDefault(1), () => objectEnumerator.Key);
                });
                
                objectEnumerator.Index = 0;
                var targetType = target.GetType();
                var keysProperty = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault(o => o.Name.EndsWith("Keys"));
                if (keysProperty?.GetValue(target) is IEnumerable keys)
                {
                    var getItemMethodInfo = targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                        .FirstOrDefault(o => o.Name == "get_Item");
                    
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
                    binder(parameters.ElementAtOrDefault(0), () => objectEnumerator.Value);
                    binder(parameters.ElementAtOrDefault(1), () => objectEnumerator.Key);
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
                binder(parameters.ElementAtOrDefault(0), () => iterator.Value);
                binder(parameters.ElementAtOrDefault(1), () => iterator.Index);
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

