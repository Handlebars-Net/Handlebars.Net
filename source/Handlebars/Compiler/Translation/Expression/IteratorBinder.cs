using System;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Reflection;

namespace HandlebarsDotNet.Compiler
{
    internal class TypeDescriptors
    {
        private static readonly Lazy<TypeDescriptors> Instance = new Lazy<TypeDescriptors>(() => new TypeDescriptors());
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, TypeDescriptor>> _descriptors = new ConcurrentDictionary<Type, ConcurrentDictionary<string, TypeDescriptor>>();

        public static TypeDescriptors Provider => Instance.Value;
        
        private TypeDescriptors(){}

        public bool TryGetGenericDictionaryTypeDescriptor(Type type, out DictionaryTypeDescriptor typeDescriptor)
        {
            var descriptors = _descriptors.GetOrAdd(type, t => new ConcurrentDictionary<string, TypeDescriptor>());
            typeDescriptor = (DictionaryTypeDescriptor) descriptors.GetOrAdd(nameof(TryGetGenericDictionaryTypeDescriptor), name =>
            {
                var typeDefinition = type.GetInterfaces()
                    .Where(i => i.GetTypeInfo().IsGenericType)
                    .FirstOrDefault(i => i.GetGenericTypeDefinition() == typeof(IDictionary<,>));

                return typeDefinition == null 
                    ? null 
                    : new DictionaryTypeDescriptor(typeDefinition);
            });

            return typeDescriptor != null;
        }

        public ObjectTypeDescriptor GetObjectTypeDescriptor(Type type)
        {
            var descriptors = _descriptors.GetOrAdd(type, t => new ConcurrentDictionary<string, TypeDescriptor>());
            return (ObjectTypeDescriptor) descriptors.GetOrAdd(nameof(GetObjectTypeDescriptor), name => new ObjectTypeDescriptor(type));
        }

        public class DictionaryTypeDescriptor : TypeDescriptor
        {
            public DictionaryTypeDescriptor(Type type) : base(type)
            {
                DictionaryAccessor = typeof(DictionaryAccessor<,>).MakeGenericType(type.GenericTypeArguments);
            }
            
            public Type DictionaryAccessor { get; }
        }
        
        public class ObjectTypeDescriptor : TypeDescriptor
        {
            private Dictionary<string, Func<object, object>> _accessors = new Dictionary<string, Func<object, object>>(StringComparer.OrdinalIgnoreCase);
            
            public ObjectTypeDescriptor(Type type) : base(type)
            {
                var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

                Members = properties.OfType<MemberInfo>().Concat(fields).ToArray();

                foreach (var member in fields)
                {
                    _accessors[member.Name] = o => member.GetValue(o);
                }
                
                foreach (var member in properties)
                {
                    _accessors[member.Name] = GetValueGetter(member);
                }
            }
            
            public MemberInfo[] Members { get; }

            public IReadOnlyDictionary<string, Func<object, object>> Accessors => _accessors;
            
            private static Func<object, object> GetValueGetter(PropertyInfo propertyInfo)
            {
                var instance = Expression.Parameter(typeof(object), "i");
                var property = Expression.Property(Expression.Convert(instance, propertyInfo.DeclaringType), propertyInfo);
                var convert = Expression.TypeAs(property, typeof(object));

                return (Func<object, object>)Expression.Lambda(convert, instance).Compile();
            }
        }
        
        public abstract class TypeDescriptor
        {
            public Type Type { get; }

            public TypeDescriptor(Type type)
            {
                Type = type;
            }
        }
    }
    
    internal class DictionaryAccessor<TKey, TValue> : IReadOnlyDictionary<object, object>
    {
        private readonly IDictionary<TKey, TValue> _wrapped;

        public DictionaryAccessor(IDictionary<TKey, TValue> wrapped)
        {
            _wrapped = wrapped;
        }

        public int Count => _wrapped.Count;
        public bool ContainsKey(object key)
        {
            return _wrapped.ContainsKey((TKey) key);
        }

        public bool TryGetValue(object key, out object value)
        {
            if(_wrapped.TryGetValue((TKey) key, out var inner))
            {
                value = inner;
                return true;
            }

            value = null;
            return false;
        }

        public object this[object key] => _wrapped[(TKey) key];

        public IEnumerable<object> Keys => _wrapped.Keys.Cast<object>();
        public IEnumerable<object> Values => _wrapped.Values.Cast<object>();
            
        public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            return _wrapped.Select(value => new KeyValuePair<object, object>(value.Key, value.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    
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
            
            var iteratorBindingContext = ExpressionShortcuts.Var<BindingContext>("context");
            var blockParamsValueBinder = ExpressionShortcuts.Var<BlockParamsValueProvider>("blockParams");
            var sequence = ExpressionShortcuts.Var<object>("sequence");
            
            var template = ExpressionShortcuts.Arg(fb.Compile(new[] {iex.Template}, iteratorBindingContext));
            var ifEmpty = ExpressionShortcuts.Arg(fb.Compile(new[] {iex.IfEmpty}, CompilationContext.BindingContext));
            
            var context = CompilationContext.BindingContext;
            var compiledSequence = FunctionBuilder.Reduce(iex.Sequence, CompilationContext);
            var blockParamsProvider = ExpressionShortcuts.New(() => new BlockParamsValueProvider(iteratorBindingContext, ExpressionShortcuts.Arg<BlockParam>(iex.BlockParams)));


            return ExpressionShortcuts.Block()
                .Parameter(iteratorBindingContext, context)
                .Parameter(blockParamsValueBinder, blockParamsProvider)
                .Parameter(sequence, compiledSequence)
                .Line(Expression.IfThenElse(
                    sequence.Is<IEnumerable>(),
                    Expression.IfThenElse(
                        ExpressionShortcuts.Call(() => IsGenericDictionary(sequence)),
                        GetDictionaryIterator(iteratorBindingContext, blockParamsValueBinder, sequence, template, ifEmpty),
                        Expression.IfThenElse(
                            ExpressionShortcuts.Call(() => IsNonListDynamic(sequence)),
                            GetDynamicIterator(iteratorBindingContext, blockParamsValueBinder, sequence, template, ifEmpty),
                            Expression.IfThenElse(ExpressionShortcuts.Call(() => IsList(sequence)),
                                GetListIterator(iteratorBindingContext, blockParamsValueBinder, sequence, template, ifEmpty),
                                GetEnumerableIterator(iteratorBindingContext, blockParamsValueBinder, sequence, template, ifEmpty)
                                )
                            )
                        ),
                    GetObjectIterator(iteratorBindingContext, blockParamsValueBinder, sequence, template, ifEmpty)));
        }

        private static Expression GetEnumerableIterator(
            ExpressionContainer<BindingContext> contextParameter, 
            ExpressionContainer<BlockParamsValueProvider> blockParamsParameter, 
            ExpressionContainer<object> sequence, 
            ExpressionContainer<Action<TextWriter, object>> template, 
            ExpressionContainer<Action<TextWriter, object>> ifEmpty
        )
        {    
            return ExpressionShortcuts.Call(
                () => IterateEnumerable(contextParameter, blockParamsParameter, (IEnumerable) sequence, template, ifEmpty)
                );
        }
        
        private static Expression GetListIterator(
            ExpressionContainer<BindingContext> contextParameter, 
            ExpressionContainer<BlockParamsValueProvider> blockParamsParameter, 
            ExpressionContainer<object> sequence, 
            ExpressionContainer<Action<TextWriter, object>> template, 
            ExpressionContainer<Action<TextWriter, object>> ifEmpty
        )
        {    
            return ExpressionShortcuts.Call(
                () => IterateList(contextParameter, blockParamsParameter, (IList) sequence, template, ifEmpty)
            );
        }

        private static Expression GetObjectIterator(
            ExpressionContainer<BindingContext> contextParameter, 
            ExpressionContainer<BlockParamsValueProvider> blockParamsParameter, 
            ExpressionContainer<object> sequence, 
            ExpressionContainer<Action<TextWriter, object>> template, 
            ExpressionContainer<Action<TextWriter, object>> ifEmpty
        )
        {
            return ExpressionShortcuts.Call(
                () => IterateObject(contextParameter, blockParamsParameter, sequence, template, ifEmpty)
                );
        }

        private static Expression GetDictionaryIterator(
            ExpressionContainer<BindingContext> contextParameter, 
            ExpressionContainer<BlockParamsValueProvider> blockParamsParameter, 
            ExpressionContainer<object> sequence, 
            ExpressionContainer<Action<TextWriter, object>> template, 
            ExpressionContainer<Action<TextWriter, object>> ifEmpty
        )
        {
            return ExpressionShortcuts.Call(
                () => IterateDictionary(contextParameter, blockParamsParameter, (IEnumerable) sequence, template, ifEmpty)
                );
        }

        private static Expression GetDynamicIterator(
            ExpressionContainer<BindingContext> contextParameter, 
            ExpressionContainer<BlockParamsValueProvider> blockParamsParameter, 
            ExpressionContainer<object> sequence, 
            ExpressionContainer<Action<TextWriter, object>> template, 
            ExpressionContainer<Action<TextWriter, object>> ifEmpty)
        {
            return ExpressionShortcuts.Call(
                () => IterateDynamic(contextParameter, blockParamsParameter, (IDynamicMetaObjectProvider) sequence, template, ifEmpty)
                );
        }

        private static bool IsList(object target)
        {
            return target is IList;
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
            return TypeDescriptors.Provider.TryGetGenericDictionaryTypeDescriptor(target.GetType(), out _);
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
                var typeDescriptor = TypeDescriptors.Provider.GetObjectTypeDescriptor(target.GetType());
                var accessorsCount = typeDescriptor.Accessors.Count;
                foreach (var accessor in typeDescriptor.Accessors)
                {
                    objectEnumerator.Key = accessor.Key;
                    objectEnumerator.Value = accessor.Value.Invoke(target);
                    objectEnumerator.First = objectEnumerator.Index == 0;
                    objectEnumerator.Last = objectEnumerator.Index == accessorsCount - 1;

                    template(context.TextWriter, objectEnumerator.Value);
                    
                    objectEnumerator.Index++;
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
                TypeDescriptors.Provider.TryGetGenericDictionaryTypeDescriptor(target.GetType(), out var typeDescriptor);
                var accessor = (IReadOnlyDictionary<object, object>) Activator.CreateInstance(typeDescriptor.DictionaryAccessor, target);
                
                var objectEnumerator = new ObjectEnumeratorValueProvider();
                context.RegisterValueProvider(objectEnumerator);
                blockParamsValueProvider.Configure((parameters, binder) =>
                {
                    binder(parameters.ElementAtOrDefault(0), () => objectEnumerator.Value);
                    binder(parameters.ElementAtOrDefault(1), () => objectEnumerator.Key);
                });
                
                objectEnumerator.Index = 0;
                var keys = accessor.Keys;
                foreach (var enumerableValue in new ExtendedEnumerable<object>(keys))
                {
                    var key = enumerableValue.Value;
                    objectEnumerator.Key = key.ToString();
                    objectEnumerator.Value = accessor[key];
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
        
        private static void IterateList(
            BindingContext context,
            BlockParamsValueProvider blockParamsValueProvider,
            IList sequence,
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
            var sequenceCount = sequence.Count;
            for (var index = 0; index < sequenceCount; index++)
            {
                iterator.Value = sequence[index];
                iterator.First = index == 0;
                iterator.Last = index == sequenceCount - 1;
                iterator.Index = index;

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
            
            public ValueTypes SupportedValueTypes { get; } = ValueTypes.Context;

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

