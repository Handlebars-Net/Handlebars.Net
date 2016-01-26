using System;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Collections.Generic;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors;

namespace HandlebarsDotNet.Compiler
{
    internal class PathBinder : HandlebarsExpressionVisitor
    {
        private static readonly List<IMemberAccessor> _accessors;

        static PathBinder()
        {
            _accessors = new List<IMemberAccessor>()
            {
                new NullInstanceMemberAccessor(),
                new EnumerableMemberAccessor(),
                new DynamicMetaObjectProviderMemberAccessor(),
                new GenericDictionaryMemberAccessor(),
                new DictionaryMemberAccessor(),
                new ObjectMemberMemberAccessor(),
            };
        }

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
            var resolvedMemberName = ResolveMemberName(instance, memberName);

            foreach (var accessor in _accessors)
            {
                var mn = accessor.RequiresResolvedMemberName ? resolvedMemberName : memberName;

                if (accessor.CanHandle(instance, mn))
                {
                    var value = accessor.AccessMember(instance, mn);
                    return value;
                }
            }

            return new UndefinedBindingResult();
        }

        private string ResolveMemberName(object instance, string memberName)
        {
            var resolver = this.CompilationContext.Configuration.ExpressionNameResolver;
            return resolver != null ? resolver.ResolveExpressionName(instance, memberName) : memberName;
        }
    }
}

