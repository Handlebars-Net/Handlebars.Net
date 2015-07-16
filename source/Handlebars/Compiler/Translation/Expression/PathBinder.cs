﻿using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.IO;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

        //TODO: make path resolution logic smarter
        private object ResolvePath(BindingContext context, string path)
        {
            var instance = context.Value;
            foreach (var segment in path.Split ('/'))
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
                else if (segment == "this")
                {
                    continue;
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
            if (segment.StartsWith("@"))
            {
                var contextValue = context.GetContextVariable(segment.Substring(1));
                if (contextValue == null)
                {
                    return new UndefinedBindingResult();
                }
                else
                {
                    return contextValue;
                }
            }
            else
            {
                return AccessMember(instance, segment);
            }
        }

        private static readonly Regex IndexRegex = new Regex(@"^\[?(?<index>\d+)\]?$", RegexOptions.Compiled);

        private object AccessMember(object instance, string memberName)
        {
            var enumerable = instance as IEnumerable<object>;
            if (enumerable != null)
            {
                int index = 0;
                var match = IndexRegex.Match(memberName);
                if (match.Success == true)
                {
                    if (match.Groups["index"].Success == false || int.TryParse(match.Groups["index"].Value, out index) == false)
                    {
                        return new UndefinedBindingResult();
                    }
                    else
                    {
                        var result = enumerable.ElementAtOrDefault(index);
                        if(result != null)
                        {
                            return result;
                        }
                        else
                        {
                            return new UndefinedBindingResult();
                        }
                    }
                }
            }
            var resolvedMemberName = this.ResolveMemberName(memberName);
            var instanceType = instance.GetType();
            //crude handling for dynamic objects that don't have metadata
            if (typeof(IDynamicMetaObjectProvider).IsAssignableFrom(instanceType))
            {
                //TODO: handle without try/catch
                try
                {
                    return GetProperty(instance, resolvedMemberName);
                }
                catch (Exception)
                {
                    return new UndefinedBindingResult();
                }
            }
            
            var iDictInstance = instanceType.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType
                        &&
                        (
                        i.GetGenericTypeDefinition() == typeof(IDictionary<,>)
#if NET45
                        || i.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>)
#endif
                        )
                    );
            if (iDictInstance != null)
            {
                var genericArgs = iDictInstance.GetGenericArguments();
                if (genericArgs.Length > 0)
                {
                    var keyName = resolvedMemberName.Trim('[', ']');    // Ensure square brackets removed.
                    var key = Convert.ChangeType(keyName, genericArgs[0]);
                    return instanceType.GetMethod("get_Item").Invoke(instance, new object[] { key });
                }
                return instanceType.GetMethod("get_Item").Invoke(instance, new object[] { resolvedMemberName });
            }
            if (instance is IDictionary)
            {
                if (((IDictionary)instance).Contains(resolvedMemberName))
                {
                    return ((IDictionary)instance)[resolvedMemberName];
                }   
            }
            var members = instanceType.GetMember(resolvedMemberName);
            if (members.Length == 0)
            {
                return new UndefinedBindingResult();
            }
            if (members[0].MemberType == System.Reflection.MemberTypes.Property)
            {
                return ((PropertyInfo)members[0]).GetValue(instance, null);
            }
            else if (members[0].MemberType == System.Reflection.MemberTypes.Field)
            {
                return ((FieldInfo)members[0]).GetValue(instance);
            }
            else
            {
                return new UndefinedBindingResult();
            }
        }
        
        private static object GetProperty(object target, string name)
        {
            var site = System.Runtime.CompilerServices.CallSite<Func<System.Runtime.CompilerServices.CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, name, target.GetType(), new[]{ Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) }));
            return site.Target(site, target);
        }

        private string ResolveMemberName(string memberName)
        {
            var resolver = this.CompilationContext.Configuration.ExpressionNameResolver;
            return resolver != null ? resolver.ResolveExpressionName(memberName) : memberName;
        }
    }
}

