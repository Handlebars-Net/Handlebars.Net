using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.IO;
using System.Dynamic;
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
						try
						{
                            instance = this.ResolveValue(context, instance, memberName);
	                    }
	                    catch (Exception)
	                    {
	                    	instance = new UndefinedBindingResult();
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
                    throw new HandlebarsRuntimeException("Couldn't bind to context variable");
                }
                return contextValue;
            }
            else
            {
                return AccessMember(instance, segment);
            }
        }

		//private static readonly Regex IndexRegex = new Regex(@"^\[?(?<index>\d+)\]?$", RegexOptions.Compiled);//todo dejand
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
                        throw new HandlebarsRuntimeException("Invalid array index in path");
                    }

	                var result = enumerable.ElementAtOrDefault(index);
	                if (result != null)
	                {
		                return result;
	                }
	                return new UndefinedBindingResult();
                }
            }
            var resolvedMemberName = this.ResolveMemberName(memberName);
            var instanceType = instance.GetType();
            //crude handling for dynamic objects that don't have metadata
            if (typeof(IDynamicMetaObjectProvider).IsAssignableFrom(instanceType))
            {
                try
                {
                    return GetProperty(instance, resolvedMemberName);
                }
                catch
				{
					return new UndefinedBindingResult();
	                //   throw new HandlebarsRuntimeException("Could not resolve dynamic member name", ex);
                }
            }
            if (instance is IDictionary)
            {
                return ((IDictionary)instance)[resolvedMemberName];
            }
            if (instanceType.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
            {
               return instanceType.GetMethod("get_Item").Invoke(instance, new object[] { resolvedMemberName });
            }
            var members = instanceType.GetMember(resolvedMemberName);
            if (members.Length != 1)
            {
                throw new InvalidOperationException("Template referenced property name that does not exist.");
            }
	        
			//// todo dejand			// i really think this should be an extension for pcl :)
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

