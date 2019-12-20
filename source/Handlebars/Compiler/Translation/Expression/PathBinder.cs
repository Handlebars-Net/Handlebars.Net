using System;
using System.Linq.Expressions;
using System.IO;
using System.Reflection;

namespace HandlebarsDotNet.Compiler
{
    internal class PathBinder : HandlebarsExpressionVisitor
    {
        private readonly MemberAccessor[] _memberAccessors;
        
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new PathBinder(context).Visit(expr);
        }

        private PathBinder(CompilationContext context)
            : base(context)
        {
            _memberAccessors = new MemberAccessor[]
            {
                new EnumerableMemberAccessor(context), 
                new DynamicObjectMemberAccessor(context), 
                new GenericDictionaryMemberAccessor(context), 
                new DictionaryMemberAccessor(context), 
                new ReflectionMemberAccessor(context)
            };
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
                    continue;
                }

                var objectPropertiesChain = containsVariable ? "@" + segment : segment;

                foreach (var memberName in objectPropertiesChain.Split('.'))
                {
                    instance = ResolveValue(context, instance, memberName);

                    if (!(instance is UndefinedBindingResult)) continue;

                    if (hashParameters == null || hashParameters.ContainsKey(memberName) || context.ParentContext == null)
                    {
                        if (CompilationContext.Configuration.ThrowOnUnresolvedBindingExpression)
                            throw new HandlebarsUndefinedBindingException(path, (instance as UndefinedBindingResult).Value);
                        return instance;
                    }

                    instance = ResolveValue(context.ParentContext, context.ParentContext.Value, memberName);
                    if (!(instance is UndefinedBindingResult undefinedBindingResult)) continue;
                    
                    if (CompilationContext.Configuration.ThrowOnUnresolvedBindingExpression)
                        throw new HandlebarsUndefinedBindingException(path, undefinedBindingResult.Value);
                        
                    return undefinedBindingResult;
                }
            }
            
            return instance;
        }

        private object ResolveValue(BindingContext context, object instance, string segment)
        {
            if (string.IsNullOrEmpty(segment) || segment == "this") return instance;
            
            if (!segment.StartsWith("@")) return AccessMember(context, instance, segment);
            
            var contextValue = context.GetContextVariable(segment.Substring(1));
            if (contextValue == null) return new UndefinedBindingResult(segment, CompilationContext.Configuration);
            if (!(contextValue is string value)) return contextValue;
                
            var member = AccessMember(context, instance, value);
            return member is UndefinedBindingResult 
                ? contextValue 
                : member;
        }

        private object AccessMember(BindingContext context, object instance, string memberName)
        {
            var undefinedBindingResult = new UndefinedBindingResult(memberName, CompilationContext.Configuration);
            
            if (instance == null) return undefinedBindingResult;

            foreach (var memberAccessor in _memberAccessors)
            {
                if (memberAccessor.TryAccessMember(context, instance, memberName, out var result))
                {
                    return result;
                }
            }

            return undefinedBindingResult;
        }
    }
}

