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
                return contextValue ?? new UndefinedBindingResult();
            }

            return AccessMember(instance, segment);
        }

        

        private object AccessMember(object instance, string memberName)
        {
            object result = null;

            var resolvedMemberName = this.ResolveMemberName(memberName);

            var instanceType = instance.GetType();

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

                var m = instanceType.GetMethods();
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

            var members = instanceType.GetMember(resolvedMemberName);
            if (members.Length != 1)
            {
                return new UndefinedBindingResult();
            }
            
            var info = members[0] as PropertyInfo;
            if (info != null)
            {
                var b = info.GetValue(instance, null);
                return b;
            }
            if (members[0] is FieldInfo)
            {
                return ((FieldInfo)members[0]).GetValue(instance);
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

        private string ResolveMemberName(string memberName)
        {
            var resolver = this.CompilationContext.Configuration.ExpressionNameResolver;
            return resolver != null ? resolver.ResolveExpressionName(memberName) : memberName;
        }
    }
}

