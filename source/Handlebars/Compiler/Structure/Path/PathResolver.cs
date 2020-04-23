using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Polyfills;

namespace HandlebarsDotNet.Compiler.Structure.Path
{
    internal static class PathResolver
    {
        public static PathInfo GetPathInfo(string path)
        {
            if (path == "null")
                return new PathInfo(false, path, null);

            var originalPath = path;
            
            var isVariable = path.StartsWith("@");
            var isInversion = path.StartsWith("^");
            var isHelper = path.StartsWith("#");
            if (isVariable || isHelper || isInversion)
            {
                path = path.Substring(1);
            }

            var segments = new List<PathSegment>();
            foreach (var segment in path.Split('/'))
            {
                if (segment == "..")
                {
                    segments.Add(new PathSegment(segment, Enumerable.Empty<ChainSegment>(), true));
                    continue;
                }
                
                var segmentString = isVariable ? "@" + segment : segment;
                segments.Add(new PathSegment(segmentString, GetPathChain(segmentString), false));
                
            }

            return new PathInfo(true, originalPath, segments);
        }
        
        
        //TODO: make path resolution logic smarter
        public static object ResolvePath(BindingContext context, ref PathInfo pathInfo)
        {
            if (!pathInfo.HasValue)
                return null;

            var configuration = context.Configuration;
            var containsVariable = pathInfo.IsVariable;
            var instance = context.Value;
            var hashParameters = instance as HashParameterDictionary;

            for (var segmentIndex = 0; segmentIndex < pathInfo.Segments.Length; segmentIndex++)
            {
                ref var segment = ref pathInfo.Segments[segmentIndex];
                if (segment.IsJumpUp)
                {
                    context = context.ParentContext;
                    if (context == null)
                    {
                        if (containsVariable) return string.Empty;

                        throw new HandlebarsCompilerException("Path expression tried to reference parent of root");
                    }

                    instance = context.Value;
                    continue;
                }

                for (var pathChainIndex = 0; pathChainIndex < segment.PathChain.Length; pathChainIndex++)
                {
                    ref var chainSegment = ref segment.PathChain[pathChainIndex];
                    instance = ResolveValue(context, instance, ref chainSegment);

                    if (!(instance is UndefinedBindingResult))
                        continue;

                    if (hashParameters == null || hashParameters.ContainsKey(chainSegment.Value) ||
                        context.ParentContext == null)
                    {
                        if (configuration.ThrowOnUnresolvedBindingExpression)
                            throw new HandlebarsUndefinedBindingException(pathInfo.Path,
                                (instance as UndefinedBindingResult).Value);
                        return instance;
                    }

                    instance = ResolveValue(context.ParentContext, context.ParentContext.Value, ref chainSegment);
                    if (!(instance is UndefinedBindingResult result)) continue;

                    if (configuration.ThrowOnUnresolvedBindingExpression)
                        throw new HandlebarsUndefinedBindingException(pathInfo.Path, result.Value);
                    return result;
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

                        list[list.Count - 1] = ChainSegment.Create($"{list[list.Count - 1].Value}.{next}");
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

                    list.Add(ChainSegment.Create(next));
                    return list;
                });
            
            return pathChain;
        }

        private static object ResolveValue(BindingContext context, object instance, ref ChainSegment chainSegment)
        {
            var configuration = context.Configuration;
            object resolvedValue;
            if (chainSegment.IsVariable)
            {
                return context.TryGetContextVariable(ref chainSegment, out resolvedValue) ? resolvedValue : new UndefinedBindingResult(chainSegment.Value, configuration);
            }

            if (chainSegment.IsThis) return instance;

            if (TryAccessMember(instance, ref chainSegment, configuration, out resolvedValue) 
                || context.TryGetVariable(ref chainSegment, out resolvedValue))
            {
                return resolvedValue;
            }

            if (chainSegment.LowerInvariant == "value" && context.TryGetVariable(ref chainSegment, out resolvedValue, true))
            {
                return resolvedValue;
            }

            return new UndefinedBindingResult(chainSegment.Value, configuration);
        }

        public static bool TryAccessMember(object instance, ref ChainSegment chainSegment, InternalHandlebarsConfiguration configuration, out object value)
        {
            var memberName = chainSegment.Value;
            if (instance == null)
            {
                value = new UndefinedBindingResult(memberName, configuration);
                return false;
            }

            var instanceType = instance.GetType();
            memberName = ResolveMemberName(instance, memberName, configuration);
            memberName = ReferenceEquals(memberName, chainSegment.Value) 
                ? chainSegment.TrimmedValue 
                : TrimSquareBrackets(memberName).Intern();

            var descriptorProvider = configuration.ObjectDescriptorProvider;
            if (!descriptorProvider.CanHandleType(instanceType))
            {
                value = new UndefinedBindingResult(memberName, configuration);
                return false;
            }

            if (!descriptorProvider.TryGetDescriptor(instanceType, out var descriptor))
            {
                value = new UndefinedBindingResult(memberName, configuration);
                return false;
            }
            
            return descriptor.MemberAccessor.TryGetValue(instance, instanceType, memberName, out value);
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

        private static string ResolveMemberName(object instance, string memberName, HandlebarsConfiguration configuration)
        {
            var resolver = configuration.ExpressionNameResolver;
            return resolver != null ? resolver.ResolveExpressionName(instance, memberName) : memberName;
        }
    }
}