using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HandlebarsDotNet.Compiler
{
    internal class PathResolver
    {
        private static readonly Regex IndexRegex = new Regex(@"^\[?(?<index>\d+)\]?$", RegexOptions.Compiled);

        private readonly HandlebarsConfiguration _configuration;

        public PathResolver(HandlebarsConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        //TODO: make path resolution logic smarter
        public object ResolvePath(BindingContext context, string path)
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
                    foreach (var memberName in GetPathChain(segmentString))
                    {
                        instance = ResolveValue(context, instance, memberName);

                        if (!(instance is UndefinedBindingResult))
                            continue;

                        if (hashParameters == null || hashParameters.ContainsKey(memberName) || context.ParentContext == null)
                        {
                            if (_configuration.ThrowOnUnresolvedBindingExpression)
                                throw new HandlebarsUndefinedBindingException(path, (instance as UndefinedBindingResult).Value);
                            return instance;
                        }

                        instance = ResolveValue(context.ParentContext, context.ParentContext.Value, memberName);
                        if (!(instance is UndefinedBindingResult result)) continue;
                        
                        if (_configuration.ThrowOnUnresolvedBindingExpression)
                            throw new HandlebarsUndefinedBindingException(path, result.Value);
                        return result;
                    }
                }
            }
            return instance;
        }
        
        private static IEnumerable<string> GetPathChain(string segmentString)
        {
            var insideEscapeBlock = false;
            var pathChain = segmentString.Split('.')
                .Aggregate(new List<string>(), (list, next) =>
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
                });
            
            return pathChain;
        }

        private object ResolveValue(BindingContext context, object instance, string segment)
        {
            var undefined = new UndefinedBindingResult(segment, _configuration);
            object resolvedValue = undefined;
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
                if (!TryAccessMember(instance, segment, out resolvedValue))
                {
                    resolvedValue = context.GetVariable(segment) ?? undefined;
                }
            }
            return resolvedValue;
        }

        private bool TryAccessMember(object instance, string memberName, out object value)
        {
            value = new UndefinedBindingResult(memberName, _configuration);
            if (instance == null)
                return false;

            var instanceType = instance.GetType();
            memberName = ResolveMemberName(instance, memberName);
            memberName = TrimSquareBrackets(memberName);
            
            return TryAccessContextMember(instance, memberName, out value) 
                   || TryAccessStringIndexerMember(instance, memberName, instanceType, out value)
                   || TryAccessIEnumerableMember(instance, memberName, out value)
                   || TryAccessGetValueMethod(instance, memberName, instanceType, out value)
                   || TryAccessDynamicMember(instance, memberName, out value)
                   || TryAccessIDictionaryMember(instance, memberName, out value)
                   || TryAccessMemberWithReflection(instance, memberName, instanceType, out value);
        }

        private static bool TryAccessContextMember(object instance, string memberName, out object value)
        {
            value = null;
            if (!(instance is BindingContext context)) return false;
            
            value = context.GetContextVariable(memberName);
            return value != null;
        }

        private static bool TryAccessStringIndexerMember(object instance, string memberName, Type instanceType, out object value)
        {
            value = null;
            var stringIndexPropertyGetter = GetStringIndexPropertyGetter(instanceType);
            if (stringIndexPropertyGetter == null) return false;

            try
            {
                value = stringIndexPropertyGetter.Invoke(instance, new object[] {memberName});
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        private static bool TryAccessIEnumerableMember(object instance, string memberName, out object value)
        {
            value = null;
            if (!(instance is IEnumerable<object> enumerable)) return false;
            
            var match = IndexRegex.Match(memberName);
            if (!match.Success) return false;
            if (!match.Groups["index"].Success || !int.TryParse(match.Groups["index"].Value, out var index)) return false;

            value = enumerable.ElementAtOrDefault(index);
            return true;
        }
        
        private static bool TryAccessMemberWithReflection(object instance, string memberName, Type instanceType, out object value)
        {
            switch (GetMember(memberName, instanceType))
            {
                case PropertyInfo propertyInfo:
                    value = propertyInfo.GetValue(instance, null);
                    return true;
                
                case FieldInfo fieldInfo:
                    value = fieldInfo.GetValue(instance);
                    return true;
                
                default:
                    value = null;
                    return false;
            }
        }

        private static bool TryAccessIDictionaryMember(object instance, string memberName, out object value)
        {
            value = null;
            // Check if the instance is IDictionary (ie, System.Collections.Hashtable)
            // Only string keys supported - indexer takes an object, but no nice
            // way to check if the hashtable check if it should be a different type.
            if (!(instance is IDictionary dictionary)) return false;
            {
                value = dictionary[memberName];
                return true;
            }
        }

        private static bool TryAccessGetValueMethod(object instance, string memberName, Type instanceType, out object value)
        {
            value = null;
            // Check if the instance is has TryGetValue method
            var tryGetValueMethod = GetTryGetValueMethod(instanceType);
            
            if (tryGetValueMethod == null) return false;
            
            object key = memberName;
            
            // Dictionary key type isn't a string, so attempt to convert.
            var keyType = tryGetValueMethod.GetParameters()[0].ParameterType;
            if (keyType != typeof(string))
            {
                if (!typeof(IConvertible).IsAssignableFrom(keyType))
                {
                    value = null;
                    return false;
                }

                key = Convert.ChangeType(memberName, keyType);
            }
            
            var methodParameters = new[] { key, null };
            var result = (bool) tryGetValueMethod.Invoke(instance, methodParameters);
            if (!result) return true;
            
            value = methodParameters[1];
            return true;
        }

        private static bool TryAccessDynamicMember(object instance, string memberName, out object value)
        {
            value = null;
            //crude handling for dynamic objects that don't have metadata
            if (!(instance is IDynamicMetaObjectProvider metaObjectProvider)) return false;
            
            try
            {
                value = GetProperty(metaObjectProvider, memberName);
                return value != null;
            }
            catch
            {
                return false;
            }
        }
        
        private static string TrimSquareBrackets(string key)
        {
            //Only trim a single layer of brackets.
            if (key.StartsWith("[") && key.EndsWith("]"))
            {
                return key.Substring(1, key.Length - 2);
            }

            return key;
        }

        private static MethodInfo GetTryGetValueMethod(Type type)
        {
            return type.GetMethods()
                .Where(o => o.Name == nameof(IDictionary<object, object>.TryGetValue))
                .Where(o =>
                {
                    var parameters = o.GetParameters();
                    return parameters.Length == 2 && parameters[1].IsOut && o.ReturnType == typeof(bool);
                })
                .SingleOrDefault();
        }
        
        private static MethodInfo GetStringIndexPropertyGetter(Type type)
        {
            return type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(prop => prop.Name == "Item" && prop.CanRead)
                .SingleOrDefault(prop =>
                {
                    var indexParams = prop.GetIndexParameters();
                    return indexParams.Length == 1 && indexParams.Single().ParameterType == typeof(string);
                })?.GetMethod;
        }
        
        private static MemberInfo GetMember(string memberName, Type instanceType)
        {
            var members = instanceType.GetMember(memberName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (members.Length == 0) return null;
            
            var preferredMember = members.Length > 1
                ? members.FirstOrDefault(m => m.Name == memberName) ?? members[0]
                : members[0];
            
            return preferredMember;
        }
        
        private static object GetProperty(object target, string name)
        {
            var site = System.Runtime.CompilerServices.CallSite<Func<System.Runtime.CompilerServices.CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, name, target.GetType(), new[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) }));
            return site.Target(site, target);
        }

        private string ResolveMemberName(object instance, string memberName)
        {
            var resolver = _configuration.ExpressionNameResolver;
            return resolver != null ? resolver.ResolveExpressionName(instance, memberName) : memberName;
        }
    }
}