using System.Runtime.CompilerServices;
using HandlebarsDotNet.Compiler;

namespace HandlebarsDotNet.PathStructure
{
    public static class PathResolver
    {
        public static object ResolvePath(BindingContext context, PathInfo pathInfo)
        {
            if (!pathInfo.HasValue) return null;
            
            if (pathInfo.HasContextChange)
            {
                context = ChangeContext(pathInfo, context);
                if (ReferenceEquals(context, null)) return string.Empty;
            }

            if (pathInfo.IsPureThis) return context.Value;
            
            var instance = context.Value;
            var pathChain = pathInfo.PathChain;
            
            var hashParameters = instance as HashParameterDictionary;

            for (var index = 0; index < pathChain.Length; index++)
            {
                var isVariable = index == 0 && pathInfo.IsVariable;
                if (!ProcessSegment(context, pathInfo, isVariable, pathChain[index], hashParameters, ref instance))
                {
                    return instance;
                }
            }

            return instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ProcessSegment(BindingContext context, PathInfo pathInfo, bool isVariable, ChainSegment chainSegment, HashParameterDictionary hashParameters, ref object instance)
        {
            instance = ResolveValue(isVariable, context, instance, chainSegment);
            if (!(instance is UndefinedBindingResult undefined)) return true;

            if (hashParameters == null || hashParameters.ContainsKey(chainSegment) || context.ParentContext == null)
            {
                if (context.Configuration.ThrowOnUnresolvedBindingExpression)
                    Throw.Undefined(pathInfo, undefined);

                return false; // return instance
            }

            instance = ResolveValue(isVariable, context.ParentContext, context.ParentContext.Value, chainSegment);
            if (!(instance is UndefinedBindingResult result)) return true;

            if (context.Configuration.ThrowOnUnresolvedBindingExpression)
                Throw.Undefined(pathInfo, result);

            return false; // return instance
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static BindingContext ChangeContext(PathInfo pathInfo, BindingContext context)
        {
            for (var i = 0; i < pathInfo.ContextChangeDepth; i++)
            {
                context = context!.ParentContext;
                if (context == null)
                {
                    if (!pathInfo.IsVariable) Throw.PathReferenceParentOfRoot();
                    else return null;
                }
            }
            
            return context;
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
                || TryAccessMember(instance, chainSegment, context.Configuration, out resolvedValue))
            {
                return resolvedValue;
            }
            
            if (chainSegment.IsValue && context.TryGetContextVariable(chainSegment, out resolvedValue))
            {
                return resolvedValue;
            }

            return UndefinedBindingResult.Create(chainSegment);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TryAccessMember(object instance, ChainSegment chainSegment, ICompiledHandlebarsConfiguration configuration, out object value)
        {
            if (instance == null)
            {
                value = UndefinedBindingResult.Create(chainSegment);
                return false;
            }
            
            chainSegment = ResolveMemberName(instance, chainSegment, configuration);
            
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
            public static void PathReferenceParentOfRoot() => throw new HandlebarsRuntimeException("Path expression tried to reference parent of root");
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Undefined(PathInfo pathInfo, UndefinedBindingResult undefinedBindingResult) => throw new HandlebarsUndefinedBindingException(pathInfo, undefinedBindingResult);
        }
    }
}