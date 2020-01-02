using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.IO;
using System.Dynamic;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;

namespace HandlebarsDotNet.Compiler
{
    internal class PathBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new PathBinder(context).Visit(expr);
        }

        private PathBinder(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            if (sex.Body is PathExpression)
            {
#if netstandard
                var writeMethod = typeof(TextWriter).GetRuntimeMethod("Write", new [] { typeof(object) });
#else
                var writeMethod = typeof(TextWriter).GetMethod("Write", new[] { typeof(object) });
#endif
                return Expression.Call(
                    Expression.Property(
                        CompilationContext.BindingContext,
                        "TextWriter"),
                    writeMethod, Visit(sex.Body));
            }
            else
            {
                return Visit(sex.Body);
            }
        }

        protected override Expression VisitPathExpression(PathExpression pex)
        {
            return Expression.Call(
                Expression.Constant(this),
#if netstandard
                new Func<BindingContext, string, object>(ResolvePath).GetMethodInfo(),
#else
                new Func<BindingContext, string, object>(ResolvePath).Method,
#endif
                CompilationContext.BindingContext,
                Expression.Constant(pex.Path));
        }

        //TODO: make path resolution logic smarter
        private object ResolvePath(BindingContext context, string path)
        {
            if (path == "null")
                return null;

            var containsVariable = path.StartsWith("@");
            if (containsVariable)
            {
                path = path.Substring(1);
                if (path.Contains(".."))
                {
                    context = context.ParentContext;
                }
            }

            var instance = context.Value;
            var hashParameters = instance as HashParameterDictionary;

            foreach (var segment in path.Split('/'))
            {
                if (segment == "..")
                {
                    context = context.ParentContext;
                    if (context == null)
                    {
                        if (containsVariable) return string.Empty;

                        throw new HandlebarsCompilerException("Path expression tried to reference parent of root");
                    }
                    instance = context.Value;
                }
                else
                {
                    var segmentString = containsVariable ? "@" + segment : segment;
                    var insideEscapeBlock = false;
                    var pathChain = segmentString.Split('.').Aggregate(new List<string>(), (list, next) =>
                    {
                        if (insideEscapeBlock)
                        {
                            if (next.EndsWith("]"))
                            {
                                insideEscapeBlock = false;
                            }

                            list[list.Count - 1] = list[list.Count - 1] + "." + next;
                            return list;
                        }
                        else
                        {
                            if (next.StartsWith("["))
                            {
                                insideEscapeBlock = true;
                            }

                            if (next.EndsWith("]"))
                            {
                                insideEscapeBlock = false;
                            }
                            
                            list.Add(next);
                            return list;
                        }
                    });
                    
                    foreach (var memberName in pathChain)
                    {
                        instance = ResolveValue(context, instance, memberName);

                        if (!(instance is UndefinedBindingResult))
                            continue;

                        if (hashParameters == null || hashParameters.ContainsKey(memberName) || context.ParentContext == null)
                        {
                            if (CompilationContext.Configuration.ThrowOnUnresolvedBindingExpression)
                                throw new HandlebarsUndefinedBindingException(path, (instance as UndefinedBindingResult).Value);
                            return instance;
                        }

                        instance = ResolveValue(context.ParentContext, context.ParentContext.Value, memberName);
                        if (instance is UndefinedBindingResult)
                        {
                            if (CompilationContext.Configuration.ThrowOnUnresolvedBindingExpression)
                                throw new HandlebarsUndefinedBindingException(path, (instance as UndefinedBindingResult).Value);
                            return instance;
                        }
                    }
                }
            }
            return instance;
        }

        private object ResolveValue(BindingContext context, object instance, string segment)
        {
            object resolvedValue = new UndefinedBindingResult(segment, CompilationContext.Configuration);
            if (segment.StartsWith("@"))
            {
                var contextValue = context.GetContextVariable(segment.Substring(1));
                if (contextValue != null)
                {
                    resolvedValue = contextValue;
                }
            }
            else if (segment == "this" || segment == string.Empty)
            {
                resolvedValue = instance;
            }
            else
            {
                resolvedValue = AccessMember(instance, segment);
            }
            return resolvedValue;
        }

        private static readonly Regex IndexRegex = new Regex(@"^\[?(?<index>\d+)\]?$", RegexOptions.None);
        
        private object AccessMember(object instance, string memberName)
        {
            if (instance == null)
                return new UndefinedBindingResult(memberName, CompilationContext.Configuration);
            
            var resolvedMemberName = ResolveMemberName(instance, memberName);
            var instanceType = instance.GetType();

            // Give preference to a string index getter if one exists
            var stringIndexPropertyGetter = GetStringIndexPropertyGetter(instanceType);
            if (stringIndexPropertyGetter != null)
            {
                string key = TrimSquareBrackets(resolvedMemberName); // Ensure square brackets removed
                try
                {
                    return stringIndexPropertyGetter.Invoke(instance, new object[] {key});
                }
                catch
                {
                    return new UndefinedBindingResult(key, CompilationContext.Configuration);
                }
            }

            var enumerable = instance as IEnumerable<object>;
            if (enumerable != null)
            {
                var match = IndexRegex.Match(memberName);
                if (match.Success)
                {
                    int index;
                    if (match.Groups["index"].Success == false ||
                        int.TryParse(match.Groups["index"].Value, out index) == false)
                    {
                        return new UndefinedBindingResult(memberName, CompilationContext.Configuration);
                    }

                    var result = enumerable.ElementAtOrDefault(index);

                    return result ?? new UndefinedBindingResult(memberName, CompilationContext.Configuration);
                }
            }
            
            //crude handling for dynamic objects that don't have metadata
            if (typeof(IDynamicMetaObjectProvider).IsAssignableFrom(instanceType))
            {
                try
                {
                    var key = TrimSquareBrackets(resolvedMemberName); // Ensure square brackets removed
                    var result = GetProperty(instance, key);
                    if (result == null)
                        return new UndefinedBindingResult(key, CompilationContext.Configuration);

                    return result;
                }
                catch
                {
                    return new UndefinedBindingResult(resolvedMemberName, CompilationContext.Configuration);
                }
            }


            // Check if the instance is IDictionary<,>
            var iDictInstance = FirstGenericDictionaryTypeInstance(instanceType);
            if (iDictInstance != null)
            {
                var genericArgs = iDictInstance.GetGenericArguments();
                object key = TrimSquareBrackets(resolvedMemberName); // Ensure square brackets removed
                if (genericArgs.Length > 0)
                {
                    // Dictionary key type isn't a string, so attempt to convert.
                    if (genericArgs[0] != typeof(string))
                    {
                        try
                        {
                            key = Convert.ChangeType(key, genericArgs[0], CultureInfo.CurrentCulture);
                        }
                        catch (Exception)
                        {
                            // Can't convert to key type.
                            return new UndefinedBindingResult(resolvedMemberName, CompilationContext.Configuration);
                        }
                    }
                }

                var containsKeyMethod = GetDictionaryMethod(instanceType, "ContainsKey");

                if (containsKeyMethod == null)
                {
                    throw new MethodAccessException("Method ContainsKey not found");
                }

                if ((bool)containsKeyMethod.Invoke(instance, new[] { key }))
                {
                    var itemProperty = GetDictionaryMethod(instanceType, "get_Item");
                    if (itemProperty == null)
                    {
                        throw new MethodAccessException("Property Item not found");
                    }

                    return itemProperty.Invoke(instance, new[] { key });
                }
                else
                {
                    // Key doesn't exist.
                    return new UndefinedBindingResult(resolvedMemberName, CompilationContext.Configuration);
                }
            }
            // Check if the instance is IDictionary (ie, System.Collections.Hashtable)
            if (typeof(IDictionary).IsAssignableFrom(instanceType))
            {
                var key = TrimSquareBrackets(resolvedMemberName); // Ensure square brackets removed
                // Only string keys supported - indexer takes an object, but no nice
                // way to check if the hashtable check if it should be a different type.
                return ((IDictionary)instance)[key];
            }

            var members = instanceType.GetMember(resolvedMemberName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            MemberInfo preferredMember;
            if (members.Length == 0)
            {
                return new UndefinedBindingResult(resolvedMemberName, CompilationContext.Configuration);
            }
            else if (members.Length > 1)
            {
                preferredMember = members.FirstOrDefault(m => m.Name == resolvedMemberName) ?? members[0];
            }
            else
            {
                preferredMember = members[0];
            }

            var propertyInfo = preferredMember as PropertyInfo;
            if (propertyInfo != null)
            {
                var propertyValue = propertyInfo.GetValue(instance, null);
                return propertyValue;
            }
            if (preferredMember is FieldInfo)
            {
                var fieldValue = ((FieldInfo)preferredMember).GetValue(instance);
                return fieldValue;
            }
            return new UndefinedBindingResult(resolvedMemberName, CompilationContext.Configuration);
        }

        //Only trim a single layer of brackets.
        private static string TrimSquareBrackets(string key)
        {
            if (key.StartsWith("[") && key.EndsWith("]"))
            {
                return key.Substring(1, key.Length - 2);
            }

            return key;
        }

        private static MethodInfo GetStringIndexPropertyGetter(Type type)
        {
            return type
                   .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                   .Where(prop => prop.Name == "Item" && prop.CanRead)
                   .SingleOrDefault(prop =>
                   {
                       var indexParams = prop.GetIndexParameters();
                       if (indexParams.Length == 1 && indexParams.Single().ParameterType == typeof(string))
                       {
                           return true;
                       }

                       return false;
                   })?.GetMethod;
        }


        private static MethodInfo GetDictionaryMethod(Type instanceType, string methodName)
        {
            var methodInfo = instanceType.GetMethod(methodName);

            if (methodInfo == null)
            {
                // Support implicit interface impl.
                methodInfo = instanceType.GetTypeInfo().DeclaredMethods.FirstOrDefault(m => m.IsPrivate && m.Name.StartsWith("System.Collections.Generic.IDictionary") && m.Name.EndsWith(methodName));
            }

            return methodInfo;
        }

        static Type FirstGenericDictionaryTypeInstance(Type instanceType)
        {
            return instanceType.GetInterfaces()
                .FirstOrDefault(i =>
#if netstandard
                    i.GetTypeInfo().IsGenericType
#else
                    i.IsGenericType
#endif
                    &&
                    (
                        i.GetGenericTypeDefinition() == typeof(IDictionary<,>)
                    )
                );
        }

        private static object GetProperty(object target, string name)
        {
            var site = System.Runtime.CompilerServices.CallSite<Func<System.Runtime.CompilerServices.CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, name, target.GetType(), new[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) }));
            return site.Target(site, target);
        }

        private string ResolveMemberName(object instance, string memberName)
        {
            var resolver = CompilationContext.Configuration.ExpressionNameResolver;
            return resolver != null ? resolver.ResolveExpressionName(instance, memberName) : memberName;
        }
    }
}

