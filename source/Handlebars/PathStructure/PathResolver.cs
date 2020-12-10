using System.Runtime.CompilerServices;
using HandlebarsDotNet.Compiler;

namespace HandlebarsDotNet.PathStructure
{
    public static class PathResolver
    {
        public static object ResolvePath(BindingContext context, PathInfo pathInfo)
        {
            if (!pathInfo.HasValue) return null;
            if (pathInfo.IsPureThis) return context.Value;
            
            var instance = context.Value;
            var throwOnUnresolvedBindingExpression = context!.Configuration.ThrowOnUnresolvedBindingExpression;

            var segments = pathInfo.Segments;
            for (var segmentIndex = 0; segmentIndex < segments.Length; segmentIndex++)
            {
                var segment = segments[segmentIndex];
                if (segment.IsThis) continue;
                if (segment.IsParent)
                {
                    context = context!.ParentContext;
                    if (context == null)
                    {
                        instance = UndefinedBindingResult.Create("..");
                        goto undefined;
                    }

                    instance = context.Value;
                    throwOnUnresolvedBindingExpression = context.Configuration.ThrowOnUnresolvedBindingExpression;
                    continue;
                }

                var pathChain = segment.PathChain;
                if (!TryResolveValue(pathInfo.IsVariable, context, pathChain[0], instance, out instance))
                {
                    instance = UndefinedBindingResult.Create(pathChain[0]);
                    goto undefined;
                }

                for (var index = 1; index < pathChain.Length; index++)
                {
                    if (TryAccessMember(context, instance, pathChain[index], out instance)) continue;
                    
                    instance = UndefinedBindingResult.Create(pathChain[index]);
                    goto undefined;
                }
            }
            
            return instance;
            
            undefined:
            if (throwOnUnresolvedBindingExpression)
            {
                Throw.Undefined(pathInfo, (UndefinedBindingResult) instance);
            }

            return instance;
        }

        private static object ResolveValue(bool isVariable, BindingContext context, object instance, ChainSegment chainSegment)
        {
            object resolvedValue;
            if (isVariable)
            {
                return context.TryGetContextVariable(chainSegment, out resolvedValue)
                    ? resolvedValue
                    : UndefinedBindingResult.Create(chainSegment);
            }

            if (chainSegment.IsThis) return instance;

            if (context.TryGetVariable(chainSegment, out resolvedValue)
                || TryAccessMember(context, instance, chainSegment, out resolvedValue))
            {
                return resolvedValue;
            }
            
            if (chainSegment.IsValue && context.TryGetContextVariable(chainSegment, out resolvedValue))
            {
                return resolvedValue;
            }

            return UndefinedBindingResult.Create(chainSegment);
        }
        
        private static bool TryResolveValue(bool isVariable, BindingContext context, ChainSegment chainSegment, object instance, out object value)
        {
            if (isVariable)
            {
                return context.TryGetContextVariable(chainSegment, out value);
            }

            if (chainSegment.IsThis)
            {
                value = context.Value;
                return true;
            }

            if (context.TryGetVariable(chainSegment, out value)
                || TryAccessMember(context, instance, chainSegment, out value))
            {
                return true;
            }
            
            return chainSegment.IsValue && context.TryGetContextVariable(chainSegment, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryAccessMember(BindingContext context, object instance, ChainSegment chainSegment, out object value)
        {
            if (instance == null)
            {
                value = null;
                return false;
            }
            
            chainSegment = ResolveMemberName(instance, chainSegment, context.Configuration);
            
            return new ObjectAccessor(instance).TryGetValue(chainSegment, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ChainSegment ResolveMemberName(object instance, ChainSegment memberName, ICompiledHandlebarsConfiguration configuration)
        {
            var resolver = configuration.ExpressionNameResolver;
            if (resolver == null) return memberName;

            return resolver.ResolveExpressionName(instance, memberName.TrimmedValue);
        }
        
        private static class Throw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Undefined(PathInfo pathInfo, UndefinedBindingResult undefinedBindingResult) => throw new HandlebarsUndefinedBindingException(pathInfo, undefinedBindingResult);
        }
    }
}