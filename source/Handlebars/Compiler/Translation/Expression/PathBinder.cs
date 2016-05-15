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

        protected override Expression VisitBlock(BlockExpression node)
        {
            return Expression.Block(
                node.Variables,
                node.Expressions.Select(expr => Visit(expr)));
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            return Expression.MakeUnary(
                node.NodeType,
                Visit(node.Operand),
                node.Type);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return Expression.Call(
                Visit(node.Object),
                node.Method,
                node.Arguments.Select(n => Visit(n)));
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            return Expression.Condition(
                Visit(node.Test),
                Visit(node.IfTrue),
                Visit(node.IfFalse));
        }

        protected override Expression VisitSubExpression(SubExpressionExpression subex)
        {
            return HandlebarsExpression.SubExpression(
                Visit(subex.Expression));
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            if (sex.Body is PathExpression)
            {
                var writeMethod = typeof(TextWriter).GetMethod("Write", new [] { typeof(object) });
                return Expression.Call(
                    Expression.Property(
                        CompilationContext.BindingContext,
                        "TextWriter"),
                    writeMethod,
                    new[] { Visit(sex.Body) });
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
                new Func<BindingContext, string, object>(ResolvePath).Method,
                CompilationContext.BindingContext,
                Expression.Constant(pex.Path));
        }

        protected override Expression VisitHelperExpression(HelperExpression hex)
        {
            return HandlebarsExpression.Helper(
                hex.HelperName,
                hex.Arguments.Select(arg => Visit(arg)));
        }

        protected override Expression VisitHashParametersExpression(HashParametersExpression hpex)
        {
            return Expression.Call(
                Expression.Constant(this),
                new Func<BindingContext, HashParametersExpression, object>(ResolveParameters).Method,
                CompilationContext.BindingContext,
                Expression.Constant(hpex));
        }

        private object ResolveParameters(BindingContext context, HashParametersExpression hpex)
        {
            var parameters = new Dictionary<string, object>();

            foreach (var parameter in hpex.Parameters)
            {
                var path = parameter.Value as PathExpression;

                if (path != null)
                {
                    parameters.Add(parameter.Key, ResolvePath(context, path.Path));
                }
                else
                {
                    parameters.Add(parameter.Key, parameter.Value);
                }
            }

            return parameters;
        }

        //TODO: make path resolution logic smarter
        private object ResolvePath(BindingContext context, string path)
        {
            var instance = context.Value;
            foreach (var segment in path.Split('/'))
            {
                if (segment == "..")
                {
                    context = context.ParentContext;
                    if (context == null)
                    {
                        throw new HandlebarsCompilerException("Path expression tried to reference parent of root");
                    }
                    instance = context.Value;
                }
                else
                {
                    foreach (var memberName in segment.Split('.'))
                    {
                        instance = this.ResolveValue(context, instance, memberName);
                        if (instance is UndefinedBindingResult)
                        {
                            break;
                        }
                    }
                }
            }
            return instance;
        }

        private object ResolveValue(BindingContext context, object instance, string segment)
        {
            object resolvedValue = new UndefinedBindingResult();
            if (segment.StartsWith("@"))
            {
                var contextValue = context.GetContextVariable(segment.Substring(1));
                if(contextValue != null)
                {
                    resolvedValue = contextValue;
                }
            }
            else if (segment == "this")
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
            var enumerable = instance as IEnumerable<object>;
            if (enumerable != null)
            {
                int index;
                var match = IndexRegex.Match(memberName);
                if (match.Success)
                {
                    if (match.Groups["index"].Success == false || int.TryParse(match.Groups["index"].Value, out index) == false)
                    {
                        return new UndefinedBindingResult();
                    }

                    var result = enumerable.ElementAtOrDefault(index);
                    
                    return result ?? new UndefinedBindingResult();
                }
            }
            var resolvedMemberName = this.ResolveMemberName(instance, memberName);
            var instanceType = instance.GetType();
            //crude handling for dynamic objects that don't have metadata
            if (typeof(IDynamicMetaObjectProvider).IsAssignableFrom(instanceType))
            {
                try
                {
                    var result =  GetProperty(instance, resolvedMemberName);
                    if (result == null)
                        return new UndefinedBindingResult();

                    return result;
                }
                catch
                {
                    return new UndefinedBindingResult();
                }
            }


            // Check if the instance is IDictionary<,>
            var iDictInstance = FirstGenericDictionaryTypeInstance(instanceType);
            if (iDictInstance != null)
            {
                var genericArgs = iDictInstance.GetGenericArguments();
                object key = resolvedMemberName.Trim('[', ']');    // Ensure square brackets removed.
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
                            return new UndefinedBindingResult();
                        }
                    }
                }

                if ((bool)instanceType.GetMethod("ContainsKey").Invoke(instance, new object[] { key }))
                {
                    return instanceType.GetMethod("get_Item").Invoke(instance, new object[] { key });
                }
                else
                {
                    // Key doesn't exist.
                    return new UndefinedBindingResult();
                }
            }
            // Check if the instance is IDictionary (ie, System.Collections.Hashtable)
            if (typeof(IDictionary).IsAssignableFrom(instanceType))
            {
                var key = resolvedMemberName.Trim('[', ']');    // Ensure square brackets removed.
                // Only string keys supported - indexer takes an object, but no nice
                // way to check if the hashtable check if it should be a different type.
                return ((IDictionary)instance)[key];
            }

            var members = instanceType.GetMember(resolvedMemberName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            MemberInfo preferredMember;
            if (members.Length == 0)
            {
                return new UndefinedBindingResult();
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
                return propertyValue ?? new UndefinedBindingResult();
            }
            if (preferredMember is FieldInfo)
            {
                var fieldValue = ((FieldInfo)preferredMember).GetValue(instance);
                return fieldValue ?? new UndefinedBindingResult();
            }
            return new UndefinedBindingResult();
        }

        static Type FirstGenericDictionaryTypeInstance(Type instanceType)
        {
            return instanceType.GetInterfaces()
                .FirstOrDefault(i =>
                    i.IsGenericType
                    &&
                    (
                        i.GetGenericTypeDefinition() == typeof(IDictionary<,>)
                    )
                );
        }

        private static object GetProperty(object target, string name)
        {
            var site = System.Runtime.CompilerServices.CallSite<Func<System.Runtime.CompilerServices.CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, name, target.GetType(), new[]{ Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) }));
            return site.Target(site, target);
        }

        private string ResolveMemberName(object instance, string memberName)
        {
            var resolver = this.CompilationContext.Configuration.ExpressionNameResolver;
            return resolver != null ? resolver.ResolveExpressionName(instance, memberName) : memberName;
        }
    }
}

