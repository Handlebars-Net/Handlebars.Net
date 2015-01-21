using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Handlebars.Compiler
{
    internal class PartialBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new PartialBinder(context).Visit(expr);
        }

        private PartialBinder(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            if (sex.Body is PartialExpression)
            {
                return Visit(sex.Body);
            }
            else
            {
                return sex;
            }
        }

        protected override Expression VisitPartialExpression(PartialExpression pex)
        {
            return Expression.Call(
                new Action<string, string, BindingContext, HandlebarsConfiguration>(InvokePartial).Method,
                Expression.Constant(pex.PartialName),
                Expression.Constant(pex.ObjectPassedIn),
                CompilationContext.BindingContext,
                Expression.Constant(CompilationContext.Configuration) );
        }

        private static void InvokePartial(string partialName,string objectPassedIn,BindingContext context, HandlebarsConfiguration configuration)
        {
            if (configuration.RegisteredTemplates.ContainsKey(partialName) == false)
            {
                throw new HandlebarsRuntimeException("Referenced partial name could not be resolved");
            }
           
            if (!string.IsNullOrEmpty(objectPassedIn))
            {
                var member = context.Value.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public).OfType<MemberInfo>()
                    .Concat(
                        context.Value.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                    ).First(x => x.Name == objectPassedIn);

                object value = AccessMember(context.Value, member);


                context = new BindingContext(value, context.TextWriter, context);
            }
            configuration.RegisteredTemplates[partialName](context.TextWriter, context);
        }

        //This is duplicated elsewhere.
        private static object AccessMember(object instance, MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Property)
            {
                return ((PropertyInfo)member).GetValue(instance, null);
            }
            else if (member.MemberType == MemberTypes.Field)
            {
                return ((FieldInfo)member).GetValue(instance);
            }
            throw new InvalidOperationException("Requested member was not a field or property");
        }
    }
}

