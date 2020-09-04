using System;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.ObjectDescriptors;
using HandlebarsDotNet.Polyfills;

namespace HandlebarsDotNet.Compiler.Structure.Path
{
    // A lot is going to be changed here in next iterations
    internal static partial class PathResolver
    {
        public static PathInfo GetPathInfo(string path)
        {
            if (path == "null")
                return new PathInfo(false, path, false, null, null);

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
                    segments.Add(new PathSegment(segment, ArrayEx.Empty<ChainSegment>(), true, null));
                    continue;
                }
                
                var segmentString = isVariable ? "@" + segment : segment;
                var chainSegments = GetPathChain(segmentString).ToArray();
                if (chainSegments.Length > 1) isValidHelperLiteral = false;
                ProcessPathChain chainDelegate;
                switch (chainSegments.Length)
                {
                    case 1: chainDelegate = ProcessPathChain_1; break;
                    case 2: chainDelegate = ProcessPathChain_2; break;
                    case 3: chainDelegate = ProcessPathChain_3; break;
                    case 4: chainDelegate = ProcessPathChain_4; break;
                    case 5: chainDelegate = ProcessPathChain_5; break;
                    default: chainDelegate = ProcessPathChain_Generic; break;
                        
                }
                
                segments.Add(new PathSegment(segmentString, chainSegments, false, chainDelegate));
            }

            ProcessSegment @delegate;
            switch (segments.Count)
            {
                case 1: @delegate = ProcessSegment_1; break;
                case 2: @delegate = ProcessSegment_2; break;
                case 3: @delegate = ProcessSegment_3; break;
                case 4: @delegate = ProcessSegment_4; break;
                case 5: @delegate = ProcessSegment_5; break;
                default: @delegate = ProcessSegment_Generic; break;
            }
            
            return new PathInfo(true, originalPath, isValidHelperLiteral, segments.ToArray(), @delegate);
        }
        
        
        //TODO: make path resolution logic smarter
        public static object ResolvePath(BindingContext context, ref PathInfo pathInfo)
        {
            if (!pathInfo.HasValue)
                return null;
            
            var instance = context.Value;
            var hashParameters = instance as HashParameterDictionary;

            return pathInfo.ProcessSegment(ref pathInfo, ref context, instance, hashParameters);
        }

        private static bool TryProcessSegment(
            ref PathInfo pathInfo, 
            ref PathSegment segment, 
            ref BindingContext context, 
            ref object instance,
            HashParameterDictionary hashParameters
            )
        {
            if (segment.IsJumpUp) return TryProcessJumpSegment(ref pathInfo, ref instance, ref context);

            instance = segment.ProcessPathChain(context, hashParameters, ref pathInfo, ref segment, instance);
            return !(instance is UndefinedBindingResult);
        }

        private static bool TryProcessJumpSegment(
            ref PathInfo pathInfo,
            ref object instance,
            ref BindingContext context
        )
        {
            context = context.ParentContext;
            if (context == null)
            {
                if (pathInfo.IsVariable)
                {
                    instance = string.Empty;
                    return false;
                }

                throw new HandlebarsCompilerException("Path expression tried to reference parent of root");
            }

            instance = context.Value;
            return true;
        }

        private static object ProcessChainSegment(
            BindingContext context, 
            HashParameterDictionary hashParameters, 
            ref PathInfo pathInfo,
            ref ChainSegment chainSegment, 
            object instance
        )
        {
            instance = ResolveValue(context, instance, ref chainSegment);

            if (!(instance is UndefinedBindingResult))
                return instance;

            if (hashParameters == null || hashParameters.ContainsKey(chainSegment) ||
                context.ParentContext == null)
            {
                if (context.Configuration.ThrowOnUnresolvedBindingExpression)
                    throw new HandlebarsUndefinedBindingException(pathInfo, (instance as UndefinedBindingResult).Value);
                return instance;
            }

            instance = ResolveValue(context.ParentContext, context.ParentContext.Value, ref chainSegment);
            if (!(instance is UndefinedBindingResult result)) return instance;

            if (context.Configuration.ThrowOnUnresolvedBindingExpression)
                throw new HandlebarsUndefinedBindingException(pathInfo, result.Value);
            return result;
        }
        
        private static IEnumerable<ChainSegment> GetPathChain(string segmentString)
        {
            var insideEscapeBlock = false;
            var pathChainParts = segmentString.Split(new[]{'.'}, StringSplitOptions.RemoveEmptyEntries);
            if (pathChainParts.Length == 0 && segmentString == ".") return new[] { new ChainSegment("this") };
            
            var pathChain = pathChainParts.Aggregate(new List<ChainSegment>(), (list, next) =>
            {
                if (insideEscapeBlock)
                {
                    if (next.EndsWith("]"))
                    {
                        insideEscapeBlock = false;
                    }

                    list[list.Count - 1] = new ChainSegment($"{list[list.Count - 1].ToString()}.{next}");
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

        private static object ResolveValue(BindingContext context, object instance, ref ChainSegment chainSegment)
        {
            object resolvedValue;
            if (chainSegment.IsVariable)
            {
                return !context.TryGetContextVariable(ref chainSegment, out resolvedValue) 
                    ? new UndefinedBindingResult(chainSegment, context.Configuration) 
                    : resolvedValue;
            }

            if (chainSegment.IsThis) return instance;

            if (TryAccessMember(instance, ref chainSegment, context.Configuration, out resolvedValue) 
                || context.TryGetVariable(ref chainSegment, out resolvedValue))
            {
                return resolvedValue;
            }

            if (chainSegment.LowerInvariant == "value" && context.TryGetVariable(ref chainSegment, out resolvedValue, true))
            {
                return resolvedValue;
            }

            return new UndefinedBindingResult(chainSegment, context.Configuration);
        }

        public static bool TryAccessMember(object instance, ref ChainSegment chainSegment, ICompiledHandlebarsConfiguration configuration, out object value)
        {
            if (instance == null)
            {
                value = new UndefinedBindingResult(chainSegment, configuration);
                return false;
            }
            
            var memberName = chainSegment.ToString();
            var instanceType = instance.GetType();
            memberName = TryResolveMemberName(instance, memberName, configuration, out var result) 
                ? ChainSegment.TrimSquareBrackets(result).Intern() 
                : chainSegment.TrimmedValue;

            if (!configuration.ObjectDescriptorProvider.CanHandleType(instanceType))
            {
                value = new UndefinedBindingResult(memberName, configuration);
                return false;
            }

            if (!configuration.ObjectDescriptorProvider.TryGetDescriptor(instanceType, out var descriptor))
            {
                value = new UndefinedBindingResult(memberName, configuration);
                return false;
            }
            
            return descriptor.MemberAccessor.TryGetValue(instance, instanceType, memberName, out value);
        }

        private static bool TryResolveMemberName(object instance, string memberName, ICompiledHandlebarsConfiguration configuration, out string value)
        {
            var resolver = configuration.ExpressionNameResolver;
            if (resolver == null)
            {
                value = null;
                return false;
            }

            value = resolver.ResolveExpressionName(instance, memberName);
            return true;
        }
    }
    
    internal static partial class PathResolver
    {
        private static object ProcessSegment_1(
            ref PathInfo pathInfo,
            ref BindingContext context,
            object instance,
            HashParameterDictionary hashParameters
        )
        {
            TryProcessSegment(ref pathInfo, ref pathInfo.Segments[0], ref context, ref instance, hashParameters);
            return instance;
        }
        
        private static object ProcessSegment_2(
            ref PathInfo pathInfo,
            ref BindingContext context,
            object instance,
            HashParameterDictionary hashParameters
        )
        {
            _ = TryProcessSegment(ref pathInfo, ref pathInfo.Segments[0], ref context, ref instance, hashParameters) &&
                TryProcessSegment(ref pathInfo, ref pathInfo.Segments[1], ref context, ref instance, hashParameters);
            return instance;
        }
        
        private static object ProcessSegment_3(
            ref PathInfo pathInfo,
            ref BindingContext context,
            object instance,
            HashParameterDictionary hashParameters
        )
        {
            _ = TryProcessSegment(ref pathInfo, ref pathInfo.Segments[0], ref context, ref instance, hashParameters) &&
                TryProcessSegment(ref pathInfo, ref pathInfo.Segments[1], ref context, ref instance, hashParameters) &&
                TryProcessSegment(ref pathInfo, ref pathInfo.Segments[2], ref context, ref instance, hashParameters);
            return instance;
        }
        
        private static object ProcessSegment_4(
            ref PathInfo pathInfo,
            ref BindingContext context,
            object instance,
            HashParameterDictionary hashParameters
        )
        {
            _ = TryProcessSegment(ref pathInfo, ref pathInfo.Segments[0], ref context, ref instance, hashParameters) &&
                TryProcessSegment(ref pathInfo, ref pathInfo.Segments[1], ref context, ref instance, hashParameters) &&
                TryProcessSegment(ref pathInfo, ref pathInfo.Segments[2], ref context, ref instance, hashParameters) &&
                TryProcessSegment(ref pathInfo, ref pathInfo.Segments[3], ref context, ref instance, hashParameters);
            return instance;
        }
        
        private static object ProcessSegment_5(
            ref PathInfo pathInfo,
            ref BindingContext context,
            object instance,
            HashParameterDictionary hashParameters
        )
        {
            _ = TryProcessSegment(ref pathInfo, ref pathInfo.Segments[0], ref context, ref instance, hashParameters) &&
                TryProcessSegment(ref pathInfo, ref pathInfo.Segments[1], ref context, ref instance, hashParameters) &&
                TryProcessSegment(ref pathInfo, ref pathInfo.Segments[2], ref context, ref instance, hashParameters) &&
                TryProcessSegment(ref pathInfo, ref pathInfo.Segments[3], ref context, ref instance, hashParameters) &&
                TryProcessSegment(ref pathInfo, ref pathInfo.Segments[4], ref context, ref instance, hashParameters);
            return instance;
        }
        
        private static object ProcessSegment_Generic(
            ref PathInfo pathInfo,
            ref BindingContext context,
            object instance,
            HashParameterDictionary hashParameters
        )
        {
            for (var segmentIndex = 0; segmentIndex < pathInfo.Segments.Length; segmentIndex++)
            {
                if (!TryProcessSegment(
                    ref pathInfo,
                    ref pathInfo.Segments[segmentIndex],
                    ref context,
                    ref instance,
                    hashParameters)
                )
                {
                    return instance;
                }
            }

            return instance;
        }
        
        private static object ProcessPathChain_1(
            BindingContext context, 
            HashParameterDictionary hashParameters, 
            ref PathInfo pathInfo,
            ref PathSegment segment, 
            object instance
        )
        {
            return ProcessChainSegment(context, hashParameters, ref pathInfo, ref segment.PathChain[0], instance);
        }
        private static object ProcessPathChain_2(
            BindingContext context, 
            HashParameterDictionary hashParameters, 
            ref PathInfo pathInfo,
            ref PathSegment segment, 
            object instance
        )
        {
            instance = ProcessChainSegment(context, hashParameters, ref pathInfo, ref segment.PathChain[0], instance);
            return ProcessChainSegment(context, hashParameters, ref pathInfo, ref segment.PathChain[1], instance);
        }
        
        private static object ProcessPathChain_3(
            BindingContext context, 
            HashParameterDictionary hashParameters, 
            ref PathInfo pathInfo,
            ref PathSegment segment, 
            object instance
        )
        {
            instance = ProcessChainSegment(context, hashParameters, ref pathInfo, ref segment.PathChain[0], instance);
            instance = ProcessChainSegment(context, hashParameters, ref pathInfo, ref segment.PathChain[1], instance);
            return ProcessChainSegment(context, hashParameters, ref pathInfo, ref segment.PathChain[2], instance);
        }
        
        private static object ProcessPathChain_4(
            BindingContext context, 
            HashParameterDictionary hashParameters, 
            ref PathInfo pathInfo,
            ref PathSegment segment, 
            object instance
        )
        {
            instance = ProcessChainSegment(context, hashParameters, ref pathInfo, ref segment.PathChain[0], instance);
            instance = ProcessChainSegment(context, hashParameters, ref pathInfo, ref segment.PathChain[1], instance);
            instance = ProcessChainSegment(context, hashParameters, ref pathInfo, ref segment.PathChain[2], instance);
            return ProcessChainSegment(context, hashParameters, ref pathInfo, ref segment.PathChain[3], instance);
        }
        
        private static object ProcessPathChain_5(
            BindingContext context, 
            HashParameterDictionary hashParameters, 
            ref PathInfo pathInfo,
            ref PathSegment segment, 
            object instance
        )
        {
            instance = ProcessChainSegment(context, hashParameters, ref pathInfo, ref segment.PathChain[0], instance);
            instance = ProcessChainSegment(context, hashParameters, ref pathInfo, ref segment.PathChain[1], instance);
            instance = ProcessChainSegment(context, hashParameters, ref pathInfo, ref segment.PathChain[2], instance);
            instance = ProcessChainSegment(context, hashParameters, ref pathInfo, ref segment.PathChain[3], instance);
            return ProcessChainSegment(context, hashParameters, ref pathInfo, ref segment.PathChain[4], instance);
        }
        
        private static object ProcessPathChain_Generic(
            BindingContext context, 
            HashParameterDictionary hashParameters, 
            ref PathInfo pathInfo,
            ref PathSegment segment, 
            object instance
        )
        {
            for (var pathChainIndex = 0; pathChainIndex < segment.PathChain.Length; pathChainIndex++)
            {
                ref var chainSegment = ref segment.PathChain[pathChainIndex];
                instance = ProcessChainSegment(context, hashParameters, ref pathInfo, ref chainSegment, instance);
            }

            return instance;
        }
    }
}