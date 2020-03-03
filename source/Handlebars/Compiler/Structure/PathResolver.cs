using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HandlebarsDotNet.Compiler
{
    [DebuggerDisplay("{Path}")]
    internal class PathInfo
    {
        public bool HasValue { get; set; }
        public string Path { get; set; }
        public bool IsVariable { get; set; }
        public IList<PathSegment> Segments { get; set; } = new List<PathSegment>();
    }
        
    [DebuggerDisplay("{Segment}")]
    internal class PathSegment
    {
        public string Segment { get; }

        public PathSegment(string segment, IEnumerable<ChainSegment> chain)
        {
            Segment = segment;
            PathChain = chain.ToArray();
        }

        public bool IsSwitch { get; set; }

        public ChainSegment[] PathChain { get; }
    }

    [DebuggerDisplay("{Value}")]
    internal class ChainSegment
    {
        public ChainSegment(string value)
        {
            Value = value;
            IsVariable = value.StartsWith("@");
            IsThis = string.IsNullOrEmpty(value) || string.Equals(value, "this", StringComparison.OrdinalIgnoreCase);
            TrimmedValue = TrimSquareBrackets(value);
        }

        public string Value { get; }
        public string TrimmedValue { get; }
        public bool IsVariable { get; }
        public bool IsThis { get; }
        
        private static string TrimSquareBrackets(string key)
        {
            //Only trim a single layer of brackets.
            if (key.StartsWith("[") && key.EndsWith("]"))
            {
                return key.Substring(1, key.Length - 2);
            }

            return key;
        }
    }
    
    internal class PathResolver
    {
        private static readonly Regex IndexRegex = new Regex(@"^\[?(?<index>\d+)\]?$", RegexOptions.Compiled);

        public static PathInfo GetPathInfo(string path)
        {
            if (path == "null")
                return new PathInfo();

            var pathInfo = new PathInfo
            {
                HasValue = true, 
                Path = path, 
                IsVariable = path.StartsWith("@")
            };

            if (pathInfo.IsVariable)
            {
                path = path.Substring(1);
            }

            foreach (var segment in path.Split('/'))
            {
                if (segment == "..")
                {
                    pathInfo.Segments.Add(new PathSegment(segment, Enumerable.Empty<ChainSegment>())
                    {
                        IsSwitch = true
                    });
                    continue;
                }
                
                var segmentString = pathInfo.IsVariable ? "@" + segment : segment;
                pathInfo.Segments.Add(new PathSegment(segmentString, GetPathChain(segmentString)));
                
            }

            return pathInfo;
        }
        
        
        //TODO: make path resolution logic smarter
        public object ResolvePath(BindingContext context, PathInfo pathInfo)
        {
            if (!pathInfo.HasValue)
                return null;

            var configuration = context.Configuration;
            var containsVariable = pathInfo.IsVariable;
            var instance = context.Value;
            var hashParameters = instance as HashParameterDictionary;

            foreach (var segment in pathInfo.Segments)
            {
                if (segment.IsSwitch)
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
                    foreach (var chainSegment in segment.PathChain)
                    {
                        instance = ResolveValue(context, instance, chainSegment);

                        if (!(instance is UndefinedBindingResult))
                            continue;

                        if (hashParameters == null || hashParameters.ContainsKey(chainSegment.Value) || context.ParentContext == null)
                        {
                            if (configuration.ThrowOnUnresolvedBindingExpression)
                                throw new HandlebarsUndefinedBindingException(pathInfo.Path, (instance as UndefinedBindingResult).Value);
                            return instance;
                        }

                        instance = ResolveValue(context.ParentContext, context.ParentContext.Value, chainSegment);
                        if (!(instance is UndefinedBindingResult result)) continue;
                        
                        if (configuration.ThrowOnUnresolvedBindingExpression)
                            throw new HandlebarsUndefinedBindingException(pathInfo.Path, result.Value);
                        return result;
                    }
                }
            }
            return instance;
        }
        
        private static IEnumerable<ChainSegment> GetPathChain(string segmentString)
        {
            var insideEscapeBlock = false;
            var pathChain = segmentString.Split('.')
                .Aggregate(new List<ChainSegment>(), (list, next) =>
                {
                    if (insideEscapeBlock)
                    {
                        if (next.EndsWith("]"))
                        {
                            insideEscapeBlock = false;
                        }

                        list[list.Count - 1] = new ChainSegment($"{list[list.Count - 1].Value}.{next}");
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

                    list.Add(new ChainSegment(next));
                    return list;
                });
            
            return pathChain;
        }

        private object ResolveValue(BindingContext context, object instance, ChainSegment chainSegment)
        {
            var configuration = context.Configuration;
            var segment = chainSegment.Value;
            object undefined = new UndefinedBindingResult(segment, configuration);
            object resolvedValue = undefined;
            if (chainSegment.IsVariable)
            {
                var contextValue = context.GetContextVariable(segment);
                if (contextValue != null)
                {
                    resolvedValue = contextValue;
                }
            }
            else if (chainSegment.IsThis)
            {
                resolvedValue = instance;
            }
            else
            {
                if (!TryAccessMember(instance, chainSegment, configuration, out resolvedValue))
                {
                    resolvedValue = context.GetVariable(segment) ?? undefined;
                }
            }
            return resolvedValue;
        }

        public bool TryAccessMember(object instance, ChainSegment chainSegment, HandlebarsConfiguration configuration, out object value)
        {
            var memberName = chainSegment.Value;
            value = new UndefinedBindingResult(memberName, configuration);
            if (instance == null)
                return false;

            var instanceType = instance.GetType();
            memberName = ResolveMemberName(instance, memberName, configuration);
            memberName = ReferenceEquals(memberName, chainSegment.Value) 
                ? chainSegment.TrimmedValue 
                : TrimSquareBrackets(memberName);

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
            var typeDescriptor = TypeDescriptors.Provider.GetObjectTypeDescriptor(instanceType);
            if(typeDescriptor.Accessors.TryGetValue(memberName, out var accessor))
            {
                value = accessor(instance);
                return true;
            }

            value = null;
            return false;
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

        private static string ResolveMemberName(object instance, string memberName, HandlebarsConfiguration configuration)
        {
            var resolver = configuration.ExpressionNameResolver;
            return resolver != null ? resolver.ResolveExpressionName(instance, memberName) : memberName;
        }
    }
}