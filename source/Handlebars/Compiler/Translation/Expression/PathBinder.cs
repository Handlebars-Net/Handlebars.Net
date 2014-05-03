using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.IO;

namespace Handlebars.Compiler
{
    internal class PathBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr)
        {
            return new PathBinder().Visit(expr);
        }

        private PathBinder()
        {
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            return Expression.Block(
                node.Expressions.Select(expr => Visit(expr)));
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            if(sex.Body is PathExpression)
            {
                var writeMethod = typeof(TextWriter).GetMethod("Write", new [] { typeof(object) });
                return Expression.Call(
                    Expression.Property(
                        HandlebarsExpression.ContextAccessor(),
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
                new Func<BindingContext, string, object>(ResolvePath).Method,
                HandlebarsExpression.ContextAccessor(),
                Expression.Constant(pex.Path));
        }

        protected override Expression VisitHelperExpression(HelperExpression hex)
        {
            return HandlebarsExpression.Helper(
                hex.HelperName,
                hex.Arguments.Select(arg => Visit(arg)));
        }

        //TODO: make path resolution logic smarter
        private static object ResolvePath(BindingContext context, string path)
        {
            var instance = context.Value;
            foreach(var segment in path.Split ('/'))
            {
                if(segment == "..")
                {
                    context = context.ParentContext;
                    if(context == null)
                    {
                        throw new HandlebarsCompilerException("Path expression tried to reference parent of root");
                    }
                }
                else if(segment == "this")
                {
                    continue;
                }
                else
                {
                    foreach(var memberName in segment.Split('.'))
                    {
                        try
                        {
                            instance = AccessMember(instance, memberName);
                        }
                        catch(Exception ex)
                        {
                            throw new HandlebarsCompilerException("Path expression could not be resolved", ex);
                        }
                    }
                }
            }
            return instance;
        }

        private static object AccessMember(object instance, string memberName)
        {
            var members = instance.GetType().GetMember(memberName);
            if(members.Length == 0)
            {
                throw new InvalidOperationException("Template referenced property name that does not exist.");
            }
            if(members[0].MemberType == System.Reflection.MemberTypes.Property)
            {
                return ((PropertyInfo)members[0]).GetValue(instance, null);
            }
            else if(members[0].MemberType == System.Reflection.MemberTypes.Field)
            {
                return ((FieldInfo)members[0]).GetValue(instance);
            }
            throw new InvalidOperationException("Requested member was not a field or property");
        }
    }
}

