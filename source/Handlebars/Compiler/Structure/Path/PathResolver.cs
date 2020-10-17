using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.ObjectDescriptors;
using HandlebarsDotNet.Polyfills;

namespace HandlebarsDotNet.Compiler.Structure.Path
{
    internal static class PathResolver
    {
        public static PathInfo GetPathInfo(string path)
        {
            if (path == "null")
                return new PathInfo(false, path, false, null);

            var originalPath = path;

            var isValidHelperLiteral = true;
            var isVariable = path.StartsWith("@");
            var isInversion = path.StartsWith("^");
            var isBlockHelper = path.StartsWith("#");
            if (isVariable || isBlockHelper || isInversion)
            {
                isValidHelperLiteral = isBlockHelper || isInversion;
                path = path.Substring(1);
            }

            var segments = new List<PathSegment>();
            var pathParts = path.Split('/');
            if (pathParts.Length > 1) isValidHelperLiteral = false;
            foreach (var segment in pathParts)
            {
                if (segment == "..")
                {
                    isValidHelperLiteral = false;
                    segments.Add(new PathSegment(segment, ArrayEx.Empty<ChainSegment>()));
                    continue;
                }

                if (segment == ".")
                {
                    isValidHelperLiteral = false;
                    segments.Add(new PathSegment(segment, ArrayEx.Empty<ChainSegment>()));
                    continue;
                }

                var segmentString = isVariable ? "@" + segment : segment;
                var chainSegments = GetPathChain(segmentString).ToArray();
                if (chainSegments.Length > 1) isValidHelperLiteral = false;

                segments.Add(new PathSegment(segmentString, chainSegments));
            }

            return new PathInfo(true, originalPath, isValidHelperLiteral, segments.ToArray());
        }
        
        public static object ResolvePath(BindingContext context, PathInfo pathInfo)
        {
            if (!pathInfo.HasValue) return null;
            
            var instance = context.Value;

            if (pathInfo.HasContextChange)
            {
                for (var i = 0; i < pathInfo.ContextChangeDepth; i++)
                {
                    context = context.ParentContext;
                    if (context == null)
                    {
                        if (!pathInfo.IsVariable)
                            throw new HandlebarsCompilerException("Path expression tried to reference parent of root");
                        
                        return string.Empty;
                    }

                    instance = context.Value;
                }
            }

            if (pathInfo.IsPureThis) return instance;
            
            var hashParameters = instance as HashParameterDictionary;
            
            var pathChain = pathInfo.PathChain;
            
            for (var index = 0; index < pathChain.Length; index++)
            {
                var chainSegment = pathChain[index];
                instance = ResolveValue(context, instance, chainSegment);

                if (!(instance is UndefinedBindingResult))
                {
                    continue;
                }

                if (hashParameters == null || hashParameters.ContainsKey(chainSegment) || context.ParentContext == null)
                {
                    if (context.Configuration.ThrowOnUnresolvedBindingExpression) 
                        throw new HandlebarsUndefinedBindingException(pathInfo, (instance as UndefinedBindingResult).Value);
                    
                    return instance;
                }

                instance = ResolveValue(context.ParentContext, context.ParentContext.Value, chainSegment);
                if (!(instance is UndefinedBindingResult result))
                {
                    continue;
                }

                if (context.Configuration.ThrowOnUnresolvedBindingExpression)
                    throw new HandlebarsUndefinedBindingException(pathInfo, result.Value);
                
                return instance;
            }

            return instance;
        }

        private static IEnumerable<ChainSegment> GetPathChain(string segmentString)
        {
            var insideEscapeBlock = false;
            var pathChainParts = segmentString.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
            if (pathChainParts.Length == 0 && segmentString == ".") return new[] {ChainSegment.Create("this")};

            var pathChain = pathChainParts.Aggregate(new List<ChainSegment>(), (list, next) =>
            {
                if (insideEscapeBlock)
                {
                    if (next.EndsWith("]"))
                    {
                        insideEscapeBlock = false;
                    }

                    list[list.Count - 1] = ChainSegment.Create($"{list[list.Count - 1]}.{next}");
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

        private static object ResolveValue(BindingContext context, object instance, ChainSegment chainSegment)
        {
            object resolvedValue;
            if (chainSegment.IsVariable)
            {
                return context.TryGetContextVariable(chainSegment, out resolvedValue)
                    ? resolvedValue
                    : chainSegment.GetUndefinedBindingResult(context.Configuration);
            }

            if (chainSegment.IsThis) return instance;

            if (context.TryGetVariable(chainSegment, out resolvedValue)
                || TryAccessMember(instance, chainSegment, context.Configuration, out resolvedValue))
            {
                return resolvedValue;
            }
            
            if (chainSegment.IsValue && context.TryGetContextVariable(chainSegment, out resolvedValue))
            {
                return resolvedValue;
            }

            return chainSegment.GetUndefinedBindingResult(context.Configuration);
        }

        public static bool TryAccessMember(object instance, ChainSegment chainSegment, ICompiledHandlebarsConfiguration configuration, out object value)
        {
            if (instance == null)
            {
                value = chainSegment.GetUndefinedBindingResult(configuration);
                return false;
            }
            
            chainSegment = ResolveMemberName(instance, chainSegment, configuration);

            value = null;
            return ObjectDescriptor.TryCreate(instance, configuration, out var descriptor) 
                   && descriptor.MemberAccessor.TryGetValue(instance, chainSegment, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ChainSegment ResolveMemberName(object instance, ChainSegment memberName,
            ICompiledHandlebarsConfiguration configuration)
        {
            var resolver = configuration.ExpressionNameResolver;
            if (resolver == null) return memberName;

            return resolver.ResolveExpressionName(instance, memberName.TrimmedValue);
        }
    }
}