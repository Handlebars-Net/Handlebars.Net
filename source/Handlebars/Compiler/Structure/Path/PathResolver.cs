using System.Runtime.CompilerServices;
using HandlebarsDotNet.ObjectDescriptors;

namespace HandlebarsDotNet.Compiler.Structure.Path
{
    public static class PathResolver
    {
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

                if (!(instance is UndefinedBindingResult undefined))
                {
                    continue;
                }

                if (hashParameters == null || hashParameters.ContainsKey(chainSegment) || context.ParentContext == null)
                {
                    if (context.Configuration.ThrowOnUnresolvedBindingExpression) 
                        throw new HandlebarsUndefinedBindingException(pathInfo, undefined);
                    
                    return instance;
                }

                instance = ResolveValue(context.ParentContext, context.ParentContext.Value, chainSegment);
                if (!(instance is UndefinedBindingResult result))
                {
                    continue;
                }

                if (context.Configuration.ThrowOnUnresolvedBindingExpression)
                    throw new HandlebarsUndefinedBindingException(pathInfo, result);
                
                return instance;
            }

            return instance;
        }
        
        private static object ResolveValue(BindingContext context, object instance, ChainSegment chainSegment)
        {
            object resolvedValue;
            if (chainSegment.IsVariable)
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

        internal static bool TryAccessMember(object instance, ChainSegment chainSegment, ICompiledHandlebarsConfiguration configuration, out object value)
        {
            if (instance == null)
            {
                value = UndefinedBindingResult.Create(chainSegment);
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