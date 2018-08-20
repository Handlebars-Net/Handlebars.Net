using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace HandlebarsDotNet.Compiler
{
    internal class HashParameterBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new HashParameterBinder(context).Visit(expr);
        }

        private HashParameterBinder(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitHashParametersExpression(HashParametersExpression hpex)
        {
            var addMethod = typeof(HashParameterDictionary).GetMethod("Add", new[] { typeof(string), typeof(object) });

            var elementInits = new List<ElementInit>();

            foreach (var parameter in hpex.Parameters)
            {
                elementInits.Add(Expression.ElementInit(
                    addMethod,
                    Expression.Constant(parameter.Key),
                    Visit(parameter.Value)));
            }

            return Expression.ListInit(
                Expression.New(typeof(HashParameterDictionary).GetConstructor(new Type[0])),
                elementInits);
        }
    }
}

